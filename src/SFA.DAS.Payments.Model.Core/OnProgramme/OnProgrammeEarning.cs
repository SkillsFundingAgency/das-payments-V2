using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SFA.DAS.Payments.Model.Core.OnProgramme
{
    public class OnProgrammeEarning
    {
        public OnProgrammeEarningType Type { get; set; }
        public ReadOnlyCollection<EarningPeriod> Periods { get; set; }
    }
}