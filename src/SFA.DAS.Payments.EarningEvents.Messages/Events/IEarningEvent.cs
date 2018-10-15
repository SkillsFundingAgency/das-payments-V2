using System.Collections.Generic;
using System.Collections.ObjectModel;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    /// <summary>
    /// Common interface for apprenticeship earning events
    /// </summary>
    /// <seealso cref="SFA.DAS.Payments.Messages.Core.Events.IPaymentsEvent" />
    public interface IEarningEvent : IPaymentsEvent
    {
        short CollectionYear { get; }
        List<PriceEpisode> PriceEpisodes { get; }
    }
}
