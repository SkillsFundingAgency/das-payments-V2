using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class DataLockEventProcessor : IDataLockEventProcessor
    {
        private readonly IDataLockFailureRepository dataLockFailureRepository;
        private readonly IDataLockStatusService dataLockStatusService;
        private readonly IMapper mapper;
        private readonly IPaymentLogger paymentLogger;

        public DataLockEventProcessor(IDataLockFailureRepository dataLockFailureRepository, IDataLockStatusService dataLockStatusService, IMapper mapper, IPaymentLogger paymentLogger)
        {
            this.dataLockFailureRepository = dataLockFailureRepository;
            this.dataLockStatusService = dataLockStatusService;
            this.mapper = mapper;
            this.paymentLogger = paymentLogger;
        }

        public async Task<List<DataLockStatusChanged>> ProcessDataLockEvent(DataLockEvent dataLockEvent)
        {
            var result = new List<DataLockStatusChanged>();
            var changedToFailed = new DataLockStatusChangedToFailed {TransactionTypesAndPeriods = new Dictionary<TransactionType, List<byte>>()};
            var changedToPassed = new DataLockStatusChangedToPassed {TransactionTypesAndPeriods = new Dictionary<TransactionType, List<byte>>()};
            var failureChanged = new DataLockFailureChanged {TransactionTypesAndPeriods = new Dictionary<TransactionType, List<byte>>()};
            var failuresToDelete = new List<long>();
            var failuresToRecord = new List<DataLockFailureEntity>();

            var newFailuresGroupedByTypeAndPeriod = GetFailuresGroupedByTypeAndPeriod(dataLockEvent);
            
            var oldFailures = await dataLockFailureRepository.GetFailures(
                dataLockEvent.Ukprn,
                dataLockEvent.Learner.ReferenceNumber,
                dataLockEvent.LearningAim.FrameworkCode,
                dataLockEvent.LearningAim.PathwayCode,
                dataLockEvent.LearningAim.ProgrammeType,
                dataLockEvent.LearningAim.StandardCode,
                dataLockEvent.LearningAim.Reference,
                dataLockEvent.CollectionYear
            ).ConfigureAwait(false);

            var fullListOfKeys = newFailuresGroupedByTypeAndPeriod.Keys
                .Concat(oldFailures.Select(f => (f.TransactionType, f.DeliveryPeriod)))
                .Distinct();

            foreach (var key in fullListOfKeys)
            {
                var transactionType = key.Item1;
                var period = key.Item2;

                var oldFailureEntity = oldFailures.FirstOrDefault(f => f.TransactionType == transactionType && f.DeliveryPeriod == period);
                var oldFailure = oldFailureEntity?.Errors;
                var newFailure = newFailuresGroupedByTypeAndPeriod.ContainsKey(key) ? newFailuresGroupedByTypeAndPeriod[key] : null;

                var statusChange = dataLockStatusService.GetStatusChange(oldFailure, newFailure);

                switch (statusChange)
                {
                    case DataLockStatusChange.ChangedToFailed:
                        AddTypeAndPeriodToEvent(changedToFailed, transactionType, period);
                        failuresToRecord.Add(CreateEntity(dataLockEvent, transactionType, period, newFailure));
                        break;
                    case DataLockStatusChange.ChangedToPassed:
                        AddTypeAndPeriodToEvent(changedToPassed, transactionType, period);
                        if (oldFailureEntity != null)
                            failuresToDelete.Add(oldFailureEntity.Id);
                        break;
                    case DataLockStatusChange.FailureCodeChanged:
                        AddTypeAndPeriodToEvent(failureChanged, transactionType, period);
                        failuresToRecord.Add(CreateEntity(dataLockEvent, transactionType, period, newFailure));
                        if (oldFailureEntity != null)
                            failuresToDelete.Add(oldFailureEntity.Id);
                        break;
                }

            }

            if (changedToFailed.TransactionTypesAndPeriods.Count > 0)
                result.Add(changedToFailed);

            if (changedToPassed.TransactionTypesAndPeriods.Count > 0)
                result.Add(changedToPassed);

            if (changedToPassed.TransactionTypesAndPeriods.Count > 0)
                result.Add(failureChanged);

            // add mapping
            foreach (var dataLockStatusChanged in result)
            {
                mapper.Map(dataLockEvent, dataLockStatusChanged);
            }

            await dataLockFailureRepository.ReplaceFailures(failuresToDelete, failuresToRecord).ConfigureAwait(false);

            paymentLogger.LogDebug($"Deleted {failuresToDelete.Count} old DL failures, created {failuresToRecord.Count} new for UKPRN {dataLockEvent.Ukprn} Learner Ref {dataLockEvent.Learner.ReferenceNumber} on R{dataLockEvent.CollectionPeriod.Period:00}");

            return result;
        }

        private static DataLockFailureEntity CreateEntity(DataLockEvent dataLockEvent, TransactionType transactionType, byte deliveryPeriod, List<DataLockFailure> failures)
        {
            return new DataLockFailureEntity
            {
                Ukprn = dataLockEvent.Ukprn,
                CollectionPeriod = dataLockEvent.CollectionPeriod.Period,
                AcademicYear = dataLockEvent.CollectionYear,
                TransactionType = transactionType,
                DeliveryPeriod = deliveryPeriod,
                LearnerReferenceNumber = dataLockEvent.Learner.ReferenceNumber,
                LearnerUln = dataLockEvent.Learner.Uln,
                LearningAimFrameworkCode = dataLockEvent.LearningAim.FrameworkCode,
                LearningAimPathwayCode = dataLockEvent.LearningAim.PathwayCode,
                LearningAimProgrammeType = dataLockEvent.LearningAim.ProgrammeType,
                LearningAimReference = dataLockEvent.LearningAim.Reference,
                LearningAimStandardCode = dataLockEvent.LearningAim.StandardCode,
                Errors = failures
            };
        }

        private static void AddTypeAndPeriodToEvent(DataLockStatusChanged changedToPassed, TransactionType transactionType, byte period)
        {
            if (changedToPassed.TransactionTypesAndPeriods.TryGetValue(transactionType, out var periods))
            {
                periods.Add(period);
            }
            else
            {
                changedToPassed.TransactionTypesAndPeriods.Add(transactionType, new List<byte> {period});
            }
        }

        private static Dictionary<(TransactionType type, byte period), List<DataLockFailure>> GetFailuresGroupedByTypeAndPeriod(DataLockEvent dataLockEvent)
        {
            var result = new Dictionary<(TransactionType type, byte period), List<DataLockFailure>>();

            foreach (var onProgrammeEarning in dataLockEvent.OnProgrammeEarnings)
            {
                foreach (var period in onProgrammeEarning.Periods)
                {
                    if (result.TryGetValue(((TransactionType) onProgrammeEarning.Type, period.Period), out var failures))
                    {
                        failures.AddRange(period.DataLockFailures);
                    }
                    else
                    {
                        result.Add(((TransactionType)onProgrammeEarning.Type, period.Period), period.DataLockFailures);
                    }
                }
                
            }

            foreach (var incentiveEarning in dataLockEvent.IncentiveEarnings)
            {
                foreach (var period in incentiveEarning.Periods)
                {
                    if (result.TryGetValue(((TransactionType) incentiveEarning.Type, period.Period), out var failures))
                    {
                        failures.AddRange(period.DataLockFailures);
                    }
                    else
                    {
                        result.Add(((TransactionType)incentiveEarning.Type, period.Period), period.DataLockFailures);
                    }
                }
                
            }

            return result;
        }
    }
}
