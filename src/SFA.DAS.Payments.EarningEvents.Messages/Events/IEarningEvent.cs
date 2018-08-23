using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public interface IEarningEvent : IPaymentsEvent
    {
        long Ukprn { get; }
        Learner Learner { get; }
        LearningAim LearningAim { get; }
    }
}
