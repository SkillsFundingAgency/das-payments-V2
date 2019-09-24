using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents
{
    public class FunctionalSkillEarningResolver : IValueResolver<FunctionalSkillEarningsEvent, EarningEventModel, List<EarningEventPeriodModel>>
    {
        public List<EarningEventPeriodModel> Resolve(FunctionalSkillEarningsEvent source, EarningEventModel destination, List<EarningEventPeriodModel> destMember, ResolutionContext context)
        {
            var periods = destination.Periods ?? new List<EarningEventPeriodModel>();
            periods.AddRange(source.Earnings
                .SelectMany(earning => earning.Periods, (earning, period) => new { earning, period })
                .Select(item => new EarningEventPeriodModel
                {
                    TransactionType = (TransactionType)item.earning.Type,
                    DeliveryPeriod = item.period.Period,
                    Amount = item.period.Amount,
                    PriceEpisodeIdentifier = item.period.PriceEpisodeIdentifier,
                    SfaContributionPercentage = item.period.SfaContributionPercentage,
                    EarningEventId = source.EventId
                })
            );
            return periods;
        }
    }
}