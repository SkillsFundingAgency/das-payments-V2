using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events.OnProgramme
{
    public class OnProgrammeEarningCalculation
    {
        public EarningType EarningType { get; set; }
        public List<PriceEpisode> PriceEpisodes { get; set;}
    }
}