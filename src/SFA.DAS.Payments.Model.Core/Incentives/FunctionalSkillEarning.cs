using System.Collections.Generic;

namespace SFA.DAS.Payments.Model.Core.Incentives
{
    public class FunctionalSkillEarning
    {
        public FunctionalSkillType Type { get; set; }
        public IReadOnlyCollection<EarningPeriod> Periods { get; set; }
    }
}