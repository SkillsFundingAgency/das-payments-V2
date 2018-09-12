using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    public interface IPaymentDueEvent
    {
        long Ukprn { get; set; }
        Learner Learner { get; set; }
        LearningAim LearningAim { get; set; }
        string PriceEpisodeIdentifier { get; set; }
        decimal Amount { get; set; }
        NamedCalendarPeriod CollectionPeriod { get; set; }
        NamedCalendarPeriod DeliveryPeriod { get; set; }
        int TransactionType { get; set; }
    }
}