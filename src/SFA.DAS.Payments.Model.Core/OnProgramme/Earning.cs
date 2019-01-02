using System;
using System.Collections.ObjectModel;

namespace SFA.DAS.Payments.Model.Core.OnProgramme
{
    public class Earning
    {
        public ReadOnlyCollection<EarningPeriod> Periods { get; set; }
        public DateTime CensusDate { get; set; }
    }
}