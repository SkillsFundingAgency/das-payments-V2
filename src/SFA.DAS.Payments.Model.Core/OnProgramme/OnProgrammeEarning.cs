using System.Collections.Generic;

namespace SFA.DAS.Payments.Model.Core.OnProgramme
{
    public class OnProgrammeEarning
    {
        public OnProgrammeEarningType Type { get; set; }
        public IReadOnlyCollection<EarningPeriod> Periods { get; set; }
    }
}