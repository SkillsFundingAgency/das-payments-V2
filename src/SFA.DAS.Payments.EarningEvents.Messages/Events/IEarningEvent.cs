using System.Collections.Generic;
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
        long Ukprn { get; }
        Learner Learner { get; }
        LearningAim LearningAim { get; }
        short EarningYear { get; }
        IReadOnlyCollection<PriceEpisode> PriceEpisodes { get; }
    }
}
