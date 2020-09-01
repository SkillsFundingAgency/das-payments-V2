using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Mapping.DataLock
{
    public class FunctionalSkillEarningsNonPayablePeriodsResolver : IValueResolver<FunctionalSkillDataLockEvent, DataLockEventModel, List<DataLockEventNonPayablePeriodModel>>
    {
        public List<DataLockEventNonPayablePeriodModel> Resolve(FunctionalSkillDataLockEvent source, DataLockEventModel destination, List<DataLockEventNonPayablePeriodModel> destMember, ResolutionContext context)
        {
            var periods = destination.NonPayablePeriods ?? new List<DataLockEventNonPayablePeriodModel>();
            periods.AddRange(source.Earnings?
                    .SelectMany(earning => earning.Periods, (earning, period) => new { earning, period })
                    .Select(item => new DataLockEventNonPayablePeriodModel
                    {
                        TransactionType = (TransactionType)item.earning.Type,
                        DeliveryPeriod = item.period.Period,
                        Amount = item.period.Amount,
                        PriceEpisodeIdentifier = item.period.PriceEpisodeIdentifier,
                        SfaContributionPercentage = item.period.SfaContributionPercentage,
                        DataLockEventId = source.EventId,
                        LearningStartDate = source.LearningAim.StartDate,
                        DataLockEventNonPayablePeriodId = Guid.NewGuid(),
                        Failures = item.period.DataLockFailures?.Select(failure => new DataLockEventNonPayablePeriodFailureModel
                        {
                            ApprenticeshipId = failure.ApprenticeshipId,
                            DataLockFailure = failure.DataLockError,
                        }).ToList() ?? new List<DataLockEventNonPayablePeriodFailureModel>(),
                    }) ?? new List<DataLockEventNonPayablePeriodModel>()
            );

            periods.ForEach(period => period.Failures.ForEach(failure => failure.DataLockEventNonPayablePeriodId = period.DataLockEventNonPayablePeriodId));
            return periods;
        }
    }
}