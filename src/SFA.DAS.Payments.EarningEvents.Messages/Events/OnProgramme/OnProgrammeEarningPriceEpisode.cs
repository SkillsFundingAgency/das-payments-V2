using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events.OnProgramme
{
    public class OnProgrammeEarningPriceEpisode: PriceEpisode
    {
        public EarningType Type { get; set; }
    }
}