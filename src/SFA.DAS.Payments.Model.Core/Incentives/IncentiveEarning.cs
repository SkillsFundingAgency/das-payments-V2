using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SFA.DAS.Payments.Model.Core.Incentives
{
    public class IncentiveEarning
    {
        public IncentiveType Type { get; set; }
        public DateTime CensusDate { get; set; }
        public ReadOnlyCollection<EarningPeriod> Periods { get; set; }
    }
}