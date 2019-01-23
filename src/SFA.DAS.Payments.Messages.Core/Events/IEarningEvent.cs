using System.Collections.ObjectModel;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IEarningEvent : IPaymentsEvent
    {
        ReadOnlyCollection<PriceEpisode> PriceEpisodes { get; }
        CollectionPeriod CollectionPeriod { get; }
        short CollectionYear { get; }
    }
}
