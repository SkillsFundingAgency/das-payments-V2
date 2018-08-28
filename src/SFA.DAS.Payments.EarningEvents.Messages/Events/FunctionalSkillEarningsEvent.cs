using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class FunctionalSkillEarningsEvent : EarningEvent
    {
        public IReadOnlyCollection<FunctionalSkillEarning> Earnings { get; set; }
    }
}