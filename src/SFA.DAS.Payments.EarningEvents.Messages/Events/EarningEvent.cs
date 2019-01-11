using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    /// <summary>
    /// Base earning event
    /// </summary>
    /// <seealso cref="SFA.DAS.Payments.EarningEvents.Messages.Events.IEarningEvent" />
    [KnownType("GetInheritors")]
    public abstract class EarningEvent : PaymentsEvent, IEarningEvent
    {
        private static Type[] inheritors;

        public ReadOnlyCollection<PriceEpisode> PriceEpisodes { get; set; }

        public string CollectionYear { get; set; }

        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(EarningEvent).Assembly.GetTypes()
                       .Where(x => x.IsSubclassOf(typeof(EarningEvent)))
                       .ToArray());
        }
    }
}