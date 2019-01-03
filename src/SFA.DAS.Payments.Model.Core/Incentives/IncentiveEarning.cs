using System;
using System.Collections.ObjectModel;

namespace SFA.DAS.Payments.Model.Core.Incentives
{
    public class IncentiveEarning
    {
        public IncentiveEarningType Type { get; set; }
        public DateTime CensusDate { get; set; }
        public ReadOnlyCollection<EarningPeriod> Periods { get; set; }
    }
}