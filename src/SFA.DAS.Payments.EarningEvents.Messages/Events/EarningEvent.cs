using System;
using System.Collections.ObjectModel;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    /// <summary>
    /// Base earning event
    /// </summary>
    /// <seealso cref="SFA.DAS.Payments.EarningEvents.Messages.Events.IEarningEvent" />
    public abstract class EarningEvent : PaymentsEvent, IEarningEvent
    {
        public short EarningYear { get; set; }
        public ReadOnlyCollection<PriceEpisode> PriceEpisodes { get; set; }
        public string CollectionYear { get; set; }
    }
}