using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Model.Core.Incentives
{
    public class IncentiveEarning
    {
        public IncentiveType Type { get; set; }
        public DateTime CensusDate { get; set; }
        public IReadOnlyCollection<EarningPeriod> Periods { get; set; }
    }
}