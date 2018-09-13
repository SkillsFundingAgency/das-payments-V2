using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public interface IRequiredPayment : IPaymentsEvent
    {
        long Ukprn { get; }
        Learner Learner { get; }
        LearningAim LearningAim { get; }
        string PriceEpisodeIdentifier { get; }
        decimal Amount { get; }
        NamedCalendarPeriod CollectionPeriod { get; }
        NamedCalendarPeriod DeliveryPeriod { get; }
    }
}