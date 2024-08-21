using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IEarningEvent : IPaymentsEvent
    {
        List<PriceEpisode> PriceEpisodes { get; }
        short CollectionYear { get; }
    }
}
