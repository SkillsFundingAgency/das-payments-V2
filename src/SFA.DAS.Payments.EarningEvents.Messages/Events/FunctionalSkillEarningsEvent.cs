using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    /// <summary>
    /// Earning events for sub aims such as Maths & English
    /// </summary>
    /// <seealso cref="SFA.DAS.Payments.EarningEvents.Messages.Events.EarningEvent" />
    public class FunctionalSkillEarningsEvent : EarningEvent
    {
        /// <summary>
        /// Gets or sets the earnings.  Earnings can be 
        /// </summary>
        /// <value>
        /// The earnings.
        /// </value>
        /// <seealso cref="SFA.DAS.Payments.Model.Core.Incentives.FunctionalSkillEarning" />
        /// <seealso cref="SFA.DAS.Payments.Model.Core.Incentives.FunctionalSkillType" />
        public IReadOnlyCollection<FunctionalSkillEarning> Earnings { get; set; }
    }
}