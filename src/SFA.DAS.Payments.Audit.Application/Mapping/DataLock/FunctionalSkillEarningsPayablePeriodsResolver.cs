using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Mapping.DataLock
{
    public class FunctionalSkillEarningsPayablePeriodsResolver : IValueResolver<FunctionalSkillDataLockEvent, DataLockEventModel, List<DataLockEventPayablePeriodModel>>
    {
        public List<DataLockEventPayablePeriodModel> Resolve(FunctionalSkillDataLockEvent source, DataLockEventModel destination, List<DataLockEventPayablePeriodModel> destMember, ResolutionContext context)
        {
            var periods = destination.PayablePeriods ?? new List<DataLockEventPayablePeriodModel>();
            periods.AddRange(source.Earnings?
                    .SelectMany(earning => earning.Periods, (earning, period) => new { earning, period })
                    .Select(item => new DataLockEventPayablePeriodModel
                    {
                        TransactionType = (TransactionType)item.earning.Type,
                        DeliveryPeriod = item.period.Period,
                        Amount = item.period.Amount,
                        PriceEpisodeIdentifier = item.period.PriceEpisodeIdentifier,
                        SfaContributionPercentage = item.period.SfaContributionPercentage,
                        DataLockEventId = source.EventId,
                        LearningStartDate = source.LearningAim.StartDate,
                        ApprenticeshipId = item.period.ApprenticeshipId,
                        ApprenticeshipEmployerType = item.period.ApprenticeshipEmployerType,
                        ApprenticeshipPriceEpisodeId = item.period.ApprenticeshipPriceEpisodeId
                    }) ?? new List<DataLockEventPayablePeriodModel>()
            );
            return periods;
        }
    }
}