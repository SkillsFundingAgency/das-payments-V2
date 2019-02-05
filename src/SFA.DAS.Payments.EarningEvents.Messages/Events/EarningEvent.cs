using System.Collections.Generic;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    /// <summary>
    /// Base earning event
    /// </summary>
    public abstract class EarningEvent : PaymentsEvent, IEarningEvent
    {
        public List<PriceEpisode> PriceEpisodes { get; set; }

        public short CollectionYear { get; set; }
    }
}