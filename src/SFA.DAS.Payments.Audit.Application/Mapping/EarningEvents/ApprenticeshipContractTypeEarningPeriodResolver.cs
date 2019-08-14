using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents
{
    public class ApprenticeshipContractTypeEarningPeriodResolver : IValueResolver<ApprenticeshipContractTypeEarningsEvent, EarningEventModel, List<EarningEventPeriodModel>>
    {
        public List<EarningEventPeriodModel> Resolve(ApprenticeshipContractTypeEarningsEvent source, EarningEventModel destination, List<EarningEventPeriodModel> destMember, ResolutionContext context)
        {
            var periods = destination.Periods ?? new List<EarningEventPeriodModel>();
            periods.AddRange(source.OnProgrammeEarnings?
                .SelectMany(onProgEarning => onProgEarning.Periods, (onProgEarning, period) => new { onProgEarning, period })
                .Select(item => new EarningEventPeriodModel
                {
                    TransactionType = (TransactionType)item.onProgEarning.Type,
                    DeliveryPeriod = item.period.Period,
                    Amount = item.period.Amount,
                    PriceEpisodeIdentifier = item.period.PriceEpisodeIdentifier,
                    SfaContributionPercentage = item.period.SfaContributionPercentage,
                    EarningEventId = source.EventId
                }) ?? new List<EarningEventPeriodModel>()
            );

            periods.AddRange(source.IncentiveEarnings?
                .SelectMany(incentiveEarning => incentiveEarning.Periods, (incentiveEarning, period) => new { incentiveEarning, period })
                .Select(item => new EarningEventPeriodModel
                {
                    TransactionType = (TransactionType)item.incentiveEarning.Type,
                    DeliveryPeriod = item.period.Period,
                    Amount = item.period.Amount,
                    PriceEpisodeIdentifier = item.period.PriceEpisodeIdentifier,
                    EarningEventId = source.EventId
                }) ?? new List<EarningEventPeriodModel>()
            );
            return periods;
        }
    }
}