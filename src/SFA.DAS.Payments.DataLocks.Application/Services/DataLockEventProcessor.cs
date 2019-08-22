using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Data;
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

        public DataLockEventProcessor(IDataLockFailureRepository dataLockFailureRepository,
            IDataLockStatusService dataLockStatusService, IMapper mapper, IPaymentLogger paymentLogger)
        {
            this.dataLockFailureRepository = dataLockFailureRepository;
            this.dataLockStatusService = dataLockStatusService;
            this.mapper = mapper;
            this.paymentLogger = paymentLogger;
        }

        public async Task<List<DataLockStatusChanged>> ProcessDataLockFailure(DataLockEvent dataLockEvent)
        {
            var result = new List<DataLockStatusChanged>();
            var changedToFailed = new DataLockStatusChangedToFailed
                {TransactionTypesAndPeriods = new Dictionary<TransactionType, List<EarningPeriod>>()};
            var changedToPassed = new DataLockStatusChangedToPassed
                {TransactionTypesAndPeriods = new Dictionary<TransactionType, List<EarningPeriod>>()};
            var failureChanged = new DataLockFailureChanged
                {TransactionTypesAndPeriods = new Dictionary<TransactionType, List<EarningPeriod>>()};
            var failuresToDelete = new List<long>();
            var failuresToRecord = new List<DataLockFailureEntity>();

            var newFailuresGroupedByTypeAndPeriod = GetFailuresGroupedByTypeAndPeriod(dataLockEvent);

            using (var scope = TransactionScopeFactory.CreateSerialisableTransaction())
            {
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
                    .Distinct()
                    .ToList();

                foreach (var key in fullListOfKeys)
                {
                    var transactionType = key.Item1;
                    var period = key.Item2;

                    if (!newFailuresGroupedByTypeAndPeriod.TryGetValue(key, out var newPeriod))
                    {
                        paymentLogger.LogWarning(
                            $"Earning does not have transaction type {transactionType} for period {period} which is present in DataLockFailure. UKPRN {dataLockEvent.Ukprn}, LearnRefNumber: {dataLockEvent.Learner.ReferenceNumber}");
                        continue;
                    }

                    var oldFailureEntity = oldFailures.FirstOrDefault(f =>
                        f.TransactionType == transactionType && f.DeliveryPeriod == period);
                    var oldFailure = oldFailureEntity?.EarningPeriod.DataLockFailures;
                    var newFailure = newPeriod?.DataLockFailures;

                    var statusChange = dataLockStatusService.GetStatusChange(oldFailure, newFailure);

                    switch (statusChange)
                    {
                        case DataLockStatusChange.ChangedToFailed:
                            AddTypeAndPeriodToEvent(changedToFailed, transactionType, newPeriod);
                            failuresToRecord.Add(CreateEntity(dataLockEvent, transactionType, period, newPeriod));
                            break;
                        case DataLockStatusChange.ChangedToPassed:
                            AddTypeAndPeriodToEvent(changedToPassed, transactionType, newPeriod);
                            failuresToDelete.Add(oldFailureEntity.Id);
                            break;
                        case DataLockStatusChange.FailureChanged:
                            AddTypeAndPeriodToEvent(failureChanged, transactionType, newPeriod);
                            failuresToRecord.Add(CreateEntity(dataLockEvent, transactionType, period, newPeriod));
                            failuresToDelete.Add(oldFailureEntity.Id);
                            break;
                    }
                }

                if (changedToFailed.TransactionTypesAndPeriods.Count > 0)
                    result.Add(changedToFailed);

                if (changedToPassed.TransactionTypesAndPeriods.Count > 0)
                    result.Add(changedToPassed);

                if (failureChanged.TransactionTypesAndPeriods.Count > 0)
                    result.Add(failureChanged);

                foreach (var dataLockStatusChanged in result)
                {
                    mapper.Map(dataLockEvent, dataLockStatusChanged);
                }

                await dataLockFailureRepository.ReplaceFailures(failuresToDelete, failuresToRecord,
                    dataLockEvent.EarningEventId, dataLockEvent.EventId).ConfigureAwait(false);

                scope.Complete();

                paymentLogger.LogDebug(
                    $"Deleted {failuresToDelete.Count} old DL failures, created {failuresToRecord.Count} new for UKPRN {dataLockEvent.Ukprn} Learner Ref {dataLockEvent.Learner.ReferenceNumber} on R{dataLockEvent.CollectionPeriod.Period:00}");

                return result;
            }
        }

        public async Task<List<DataLockStatusChanged>> ProcessPayableEarning(DataLockEvent payableEarningEvent)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = new List<DataLockStatusChanged>();
                var changedToPassed = new DataLockStatusChangedToPassed
                    {TransactionTypesAndPeriods = new Dictionary<TransactionType, List<EarningPeriod>>()};
                var failuresToDelete = new List<long>();

                var newPassesGroupedByTypeAndPeriod = GetPassesGroupedByTypeAndPeriod(payableEarningEvent);

                var oldFailures = await dataLockFailureRepository.GetFailures(
                    payableEarningEvent.Ukprn,
                    payableEarningEvent.Learner.ReferenceNumber,
                    payableEarningEvent.LearningAim.FrameworkCode,
                    payableEarningEvent.LearningAim.PathwayCode,
                    payableEarningEvent.LearningAim.ProgrammeType,
                    payableEarningEvent.LearningAim.StandardCode,
                    payableEarningEvent.LearningAim.Reference,
                    payableEarningEvent.CollectionYear
                ).ConfigureAwait(false);

                foreach (var oldFailure in oldFailures)
                {
                    if (newPassesGroupedByTypeAndPeriod.TryGetValue(
                        (oldFailure.TransactionType, oldFailure.DeliveryPeriod), out var newPass))
                    {
                        AddTypeAndPeriodToEvent(changedToPassed, oldFailure.TransactionType, newPass);
                        failuresToDelete.Add(oldFailure.Id);
                    }
                }

                if (changedToPassed.TransactionTypesAndPeriods.Count > 0)
                    result.Add(changedToPassed);

                foreach (var dataLockStatusChanged in result)
                {
                    mapper.Map(payableEarningEvent, dataLockStatusChanged);
                }

                await dataLockFailureRepository.ReplaceFailures(failuresToDelete, new List<DataLockFailureEntity>(),
                    payableEarningEvent.EarningEventId, payableEarningEvent.EventId).ConfigureAwait(false);

                scope.Complete();

                paymentLogger.LogDebug(
                    $"Deleted {failuresToDelete.Count} old DL failures for UKPRN {payableEarningEvent.Ukprn} Learner Ref {payableEarningEvent.Learner.ReferenceNumber} on R{payableEarningEvent.CollectionPeriod.Period:00}");

                return result;
            }
        }

        private static DataLockFailureEntity CreateEntity(DataLockEvent dataLockEvent, TransactionType transactionType,
            byte deliveryPeriod, EarningPeriod period)
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
                EarningPeriod = period
            };
        }

        private static void AddTypeAndPeriodToEvent(DataLockStatusChanged statusChangedEvent,
            TransactionType transactionType, EarningPeriod period)
        {
            if (statusChangedEvent.TransactionTypesAndPeriods.TryGetValue(transactionType, out var periods))
            {
                periods.Add(period);
            }
            else
            {
                statusChangedEvent.TransactionTypesAndPeriods.Add(transactionType, new List<EarningPeriod> {period});
            }

            if (statusChangedEvent is DataLockStatusChangedToPassed)
            {
                if (period.DataLockFailures?.Count > 0)
                    throw new ApplicationException("DataLockStatusChangedToPassed has data lock failures");

                if (!period.ApprenticeshipId.HasValue || !period.ApprenticeshipPriceEpisodeId.HasValue)
                    throw new ApplicationException("DataLockStatusChangedToPassed has no apprenticeship ID");
            }
        }

        private Dictionary<(TransactionType type, byte period), EarningPeriod> GetFailuresGroupedByTypeAndPeriod(
            DataLockEvent dataLockEvent)
        {
            var result = new Dictionary<(TransactionType type, byte period), EarningPeriod>();

            if (dataLockEvent.OnProgrammeEarnings != null && dataLockEvent.OnProgrammeEarnings.Any())
            {
                foreach (var onProgrammeEarning in dataLockEvent.OnProgrammeEarnings)
                {
                    foreach (var period in onProgrammeEarning.Periods)
                    {
                        if (period.Amount == 0 && period.PriceEpisodeIdentifier == null)
                            continue; // DataLocks are generated for all periods, event irrelevant, ignore until fixed

                        var key = ((TransactionType) onProgrammeEarning.Type, period.Period);
                        if (result.ContainsKey(key))
                        {
                            paymentLogger.LogWarning($"DataLockEvent trying to add duplicate key \n\n " +
                                                     $"Existing Period: {JsonConvert.SerializeObject(result[key])}\n\n" +
                                                     $"Period that failed to add: {JsonConvert.SerializeObject(period)}");
                        }
                        else
                        {
                            result.Add(key, period);
                        }
                    }
                }
            }

            if (dataLockEvent.IncentiveEarnings != null && dataLockEvent.IncentiveEarnings.Any())
            {
                foreach (var incentiveEarning in dataLockEvent.IncentiveEarnings)
                {
                    foreach (var period in incentiveEarning.Periods)
                    {
                        if (period.Amount == 0 && period.PriceEpisodeIdentifier == null)
                            continue; // DataLocks are generated for all periods, event irrelevant, ignore until fixed

                        var key = ((TransactionType) incentiveEarning.Type, period.Period);
                        if (result.ContainsKey(key))
                        {
                            paymentLogger.LogWarning($"DataLockEvent trying to add duplicate key \n\n " +
                                                     $"Existing Period: {JsonConvert.SerializeObject(result[key])}\n\n" +
                                                     $"Period that failed to add: {JsonConvert.SerializeObject(period)}");
                        }
                        else
                        {
                            result.Add(key, period);
                        }
                    }
                }
            }

            return result;
        }

        private Dictionary<(TransactionType type, byte period), EarningPeriod> GetPassesGroupedByTypeAndPeriod(
            DataLockEvent dataLockEvent)
        {
            var result = new Dictionary<(TransactionType type, byte period), EarningPeriod>();

            foreach (var onProgrammeEarning in dataLockEvent.OnProgrammeEarnings)
            {
                foreach (var period in onProgrammeEarning.Periods)
                {
                    var key = ((TransactionType) onProgrammeEarning.Type, period.Period);
                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, period);
                    }
                    else
                    {
                        paymentLogger.LogWarning($"DataLockEvent trying to add duplicate key \n\n " +
                                                 $"Existing Period: {JsonConvert.SerializeObject(result[key])}\n\n" +
                                                 $"Period that failed to add: {JsonConvert.SerializeObject(period)}");
                    }
                }
            }

            foreach (var incentiveEarning in dataLockEvent.IncentiveEarnings)
            {
                foreach (var period in incentiveEarning.Periods)
                {
                    var key = ((TransactionType) incentiveEarning.Type, period.Period);
                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, period);
                    }
                    else
                    {
                        paymentLogger.LogWarning($"DataLockEvent trying to add duplicate key \n\n " +
                                                 $"Existing Period: {JsonConvert.SerializeObject(result[key])}\n\n" +
                                                 $"Period that failed to add: {JsonConvert.SerializeObject(period)}");
                    }
                }
            }

            return result;
        }
    }
}
