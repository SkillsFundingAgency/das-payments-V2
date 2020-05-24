using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Mapping.DataLock
{
    public class NonPayablePeriodResolver : IValueResolver<DataLockEvent, DataLockEventModel, List<DataLockEventNonPayablePeriodModel>>
    {
        public List<DataLockEventNonPayablePeriodModel> Resolve(DataLockEvent source, DataLockEventModel destination, List<DataLockEventNonPayablePeriodModel> destMember, ResolutionContext context)
        {
            var periods = destination.NonPayablePeriods ?? new List<DataLockEventNonPayablePeriodModel>();
            periods.AddRange(source.OnProgrammeEarnings?
                    .SelectMany(onProgEarning => onProgEarning.Periods, (onProgEarning, period) => new { onProgEarning, period })
                    .Select(item => new DataLockEventNonPayablePeriodModel
                    {
                        TransactionType = (TransactionType)item.onProgEarning.Type,
                        DeliveryPeriod = item.period.Period,
                        Amount = item.period.Amount,
                        PriceEpisodeIdentifier = item.period.PriceEpisodeIdentifier,
                        SfaContributionPercentage = item.period.SfaContributionPercentage,
                        DataLockEventId = source.EventId,
                        CensusDate = item.onProgEarning.CensusDate,
                        LearningStartDate = source.LearningAim.StartDate,
                        Failures = item.period.DataLockFailures?.Select(failure => new DataLockEventNonPayablePeriodFailureModel
                        {
                            ApprenticeshipId = failure.ApprenticeshipId,
                            DataLockFailure = failure.DataLockError
                        }).ToList(),
                    }) ?? new List<DataLockEventNonPayablePeriodModel>()
            );

            periods.AddRange(source.IncentiveEarnings?
                    .SelectMany(incentiveEarning => incentiveEarning.Periods, (incentiveEarning, period) => new { incentiveEarning, period })
                    .Select(item => new DataLockEventNonPayablePeriodModel
                    {
                        TransactionType = (TransactionType)item.incentiveEarning.Type,
                        DeliveryPeriod = item.period.Period,
                        Amount = item.period.Amount,
                        PriceEpisodeIdentifier = item.period.PriceEpisodeIdentifier,
                        SfaContributionPercentage = item.period.SfaContributionPercentage,
                        DataLockEventId = source.EventId,
                        CensusDate = item.incentiveEarning.CensusDate,
                        LearningStartDate = source.LearningAim.StartDate,
                        Failures = item.period.DataLockFailures?.Select(failure => new DataLockEventNonPayablePeriodFailureModel
                        {
                            ApprenticeshipId = failure.ApprenticeshipId,
                            DataLockFailure = failure.DataLockError
                        }).ToList(),
                    }) ?? new List<DataLockEventNonPayablePeriodModel>()
            );
            return periods;
        }
    }
}