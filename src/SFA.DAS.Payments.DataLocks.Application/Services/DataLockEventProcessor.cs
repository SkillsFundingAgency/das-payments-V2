using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class DataLockEventProcessor : IDataLockEventProcessor
    {
        private readonly IDataLockFailureRepository dataLockFailureRepository;
        private readonly IDataLockStatusService dataLockStatusService;

        public DataLockEventProcessor(IDataLockFailureRepository dataLockFailureRepository, IDataLockStatusService dataLockStatusService)
        {
            this.dataLockFailureRepository = dataLockFailureRepository;
            this.dataLockStatusService = dataLockStatusService;
        }

        public async Task<List<DataLockStatusChanged>> ProcessDataLockEvent(DataLockEvent dataLockEvent)
        {
            var result = new List<DataLockStatusChanged>();
            var changedToFailed = new DataLockStatusChangedToFailed {TransactionTypesAndPeriods = new Dictionary<byte, List<byte>>()};
            var changedToPassed = new DataLockStatusChangedToPassed {TransactionTypesAndPeriods = new Dictionary<byte, List<byte>>()};
            var failureChanged = new DataLockFailureChanged {TransactionTypesAndPeriods = new Dictionary<byte, List<byte>>()};

            var newFailuresGroupedByTypeAndPeriod = GetFailuresGroupedByTypeAndPeriod(dataLockEvent);
            
            var existingFailures = await dataLockFailureRepository.GetFailures(
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
                .Concat(existingFailures.Select(f => ((byte)f.TransactionType, f.DeliveryPeriod)))
                .Distinct();

            foreach (var key in fullListOfKeys)
            {
                var transactionType = key.Item1;
                var period = key.Item2;

                var currentFailures = existingFailures.FirstOrDefault(f => (byte)f.TransactionType == transactionType && f.DeliveryPeriod == period)?.Errors;
                var newFailures = newFailuresGroupedByTypeAndPeriod.ContainsKey(key) ? newFailuresGroupedByTypeAndPeriod[key] : null;

                var statusChange = dataLockStatusService.GetStatusChange(currentFailures, newFailures);

                switch (statusChange)
                {
                    case DataLockStatusChange.ChangedToFailed:
                        AddTypeAndPeriodToEvent(changedToFailed, transactionType, period);
                        break;
                    case DataLockStatusChange.ChangedToPassed:
                        AddTypeAndPeriodToEvent(changedToPassed, transactionType, period);
                        break;
                    case DataLockStatusChange.FailureCodeChanged:
                        AddTypeAndPeriodToEvent(failureChanged, transactionType, period);
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
                
            }

            return result;
        }

        private static void AddTypeAndPeriodToEvent(DataLockStatusChanged changedToPassed, byte transactionType, byte period)
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

        private static Dictionary<(byte type, byte period), List<DataLockFailure>> GetFailuresGroupedByTypeAndPeriod(DataLockEvent dataLockEvent)
        {
            var result = new Dictionary<(byte type, byte period), List<DataLockFailure>>();

            foreach (var onProgrammeEarning in dataLockEvent.OnProgrammeEarnings)
            {
                foreach (var period in onProgrammeEarning.Periods)
                {
                    if (result.TryGetValue(((byte) onProgrammeEarning.Type, period.Period), out var failures))
                    {
                        failures.AddRange(period.DataLockFailures);
                    }
                    else
                    {
                        result.Add(((byte)onProgrammeEarning.Type, period.Period), period.DataLockFailures);
                    }
                }
                
            }

            foreach (var incentiveEarning in dataLockEvent.IncentiveEarnings)
            {
                foreach (var period in incentiveEarning.Periods)
                {
                    if (result.TryGetValue(((byte) incentiveEarning.Type, period.Period), out var failures))
                    {
                        failures.AddRange(period.DataLockFailures);
                    }
                    else
                    {
                        result.Add(((byte)incentiveEarning.Type, period.Period), period.DataLockFailures);
                    }
                }
                
            }

            return result;
        }
    }
}
