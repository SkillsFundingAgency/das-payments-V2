using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public interface IRequiredPayment : IPaymentsEvent
    {
        long Ukprn { get; }
        Learner Learner { get; }
        LearningAim LearningAim { get; }
        byte Period { get; }
        string PriceEpisodeIdentifier { get; }
        decimal AmountDue { get; }
        NamedCalendarPeriod CollectionPeriod { get; }
        CalendarPeriod DeliveryPeriod { get; }
    }
}