using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Mapping.DataLock
{
    public class PayablePeriodResolver : IValueResolver<DataLockEvent, DataLockEventModel, List<DataLockEventPayablePeriodModel>>
    {
        public List<DataLockEventPayablePeriodModel> Resolve(DataLockEvent source, DataLockEventModel destination, List<DataLockEventPayablePeriodModel> destMember, ResolutionContext context)
        {
            var periods = destination.PayablePeriods ?? new List<DataLockEventPayablePeriodModel>();
            periods.AddRange(source.OnProgrammeEarnings?
                    .SelectMany(onProgEarning => onProgEarning.Periods, (onProgEarning, period) => new { onProgEarning, period })
                    .Select(item => new DataLockEventPayablePeriodModel
                    {
                        TransactionType = (TransactionType)item.onProgEarning.Type,
                        DeliveryPeriod = item.period.Period,
                        Amount = item.period.Amount,
                        PriceEpisodeIdentifier = item.period.PriceEpisodeIdentifier,
                        SfaContributionPercentage = item.period.SfaContributionPercentage,
                        DataLockEventId = source.EventId,
                        CensusDate = item.onProgEarning.CensusDate,
                        LearningStartDate = source.LearningAim.StartDate,
                        ApprenticeshipId = item.period.ApprenticeshipId,
                        ApprenticeshipPriceEpisodeId = item.period.ApprenticeshipPriceEpisodeId,
                        ApprenticeshipEmployerType = item.period.ApprenticeshipEmployerType,
                            
                    }) ?? new List<DataLockEventPayablePeriodModel>()
            );

            periods.AddRange(source.IncentiveEarnings?
                    .SelectMany(incentiveEarning => incentiveEarning.Periods, (incentiveEarning, period) => new { incentiveEarning, period })
                    .Select(item => new DataLockEventPayablePeriodModel
                    {
                        TransactionType = (TransactionType)item.incentiveEarning.Type,
                        DeliveryPeriod = item.period.Period,
                        Amount = item.period.Amount,
                        PriceEpisodeIdentifier = item.period.PriceEpisodeIdentifier,
                        SfaContributionPercentage = item.period.SfaContributionPercentage,
                        DataLockEventId = source.EventId,
                        CensusDate = item.incentiveEarning.CensusDate,
                        LearningStartDate = source.LearningAim.StartDate,
                        ApprenticeshipId = item.period.ApprenticeshipId,
                        ApprenticeshipPriceEpisodeId = item.period.ApprenticeshipPriceEpisodeId,
                        ApprenticeshipEmployerType = item.period.ApprenticeshipEmployerType,
                    }) ?? new List<DataLockEventPayablePeriodModel>()
            );
            return periods;
        }
    }
}