using System.Collections.ObjectModel;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public interface IEarningEvent : IPaymentsEvent
    {
        ReadOnlyCollection<PriceEpisode> PriceEpisodes { get; }
        CollectionPeriod CollectionPeriod { get; }
        string CollectionYear { get; }
    }
}
