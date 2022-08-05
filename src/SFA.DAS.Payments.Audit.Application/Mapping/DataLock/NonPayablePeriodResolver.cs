using System;
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
                        AcademicYear = source.CollectionPeriod.AcademicYear,
                        CollectionPeriod = source.CollectionPeriod.Period,
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
                            AcademicYear = source.CollectionPeriod.AcademicYear,
                            CollectionPeriod = source.CollectionPeriod.Period
                        }).ToList() ?? new List<DataLockEventNonPayablePeriodFailureModel>(),
                    }) ?? new List<DataLockEventNonPayablePeriodModel>()
            );

            periods.AddRange(source.IncentiveEarnings?
                    .SelectMany(incentiveEarning => incentiveEarning.Periods, (incentiveEarning, period) => new { incentiveEarning, period })
                    .Select(item => new DataLockEventNonPayablePeriodModel
                    {
                        TransactionType = (TransactionType)item.incentiveEarning.Type,
                        AcademicYear = source.CollectionPeriod.AcademicYear,
                        CollectionPeriod = source.CollectionPeriod.Period,
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
                            AcademicYear = source.CollectionPeriod.AcademicYear,
                            CollectionPeriod = source.CollectionPeriod.Period
                        }).ToList() ?? new List<DataLockEventNonPayablePeriodFailureModel>(),
                    }) ?? new List<DataLockEventNonPayablePeriodModel>()
            );
            periods.ForEach(period => period.Failures.ForEach(failure => failure.DataLockEventNonPayablePeriodId = period.DataLockEventNonPayablePeriodId));
            return periods;
        }
    }
}