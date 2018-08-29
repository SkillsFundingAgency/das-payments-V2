using System.Collections.Generic;

namespace SFA.DAS.Payments.Model.Core.Incentives
{
    /// <summary>
    /// Earnings for Sub Aims such as Maths & English. 
    /// </summary>
    public class FunctionalSkillEarning
    {
        /// <summary>
        /// The type of earning. Can be "On Programme Maths And English" (13) or "Balancing Maths And English" (14)
        /// </summary>
        /// <value>
        /// Can be "On Programme Maths And English" (13) or "Balancing Maths And English" (14)
        /// </value>
        /// <seealso cref="SFA.DAS.Payments.Model.Core.Incentives.FunctionalSkillType" />
        public FunctionalSkillType Type { get; set; }
        public IReadOnlyCollection<EarningPeriod> Periods { get; set; }
    }
}