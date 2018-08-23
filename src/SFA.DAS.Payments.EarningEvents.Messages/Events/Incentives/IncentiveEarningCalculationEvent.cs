using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events.Incentives
{
    public class IncentiveEarningEvent : EarningEvent
    {
        public IncentiveType Type { get; set; } 
        public List<PriceEpisode> PriceEpisodes { get; set; }
    }
}