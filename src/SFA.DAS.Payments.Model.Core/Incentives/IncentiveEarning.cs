using System.Collections.Generic;

namespace SFA.DAS.Payments.Model.Core.Incentives
{
    public class IncentiveEarning
    {
        public IncentiveType Type { get; set; }
        public IReadOnlyCollection<EarningPeriod> Periods { get; set; }
    }

}