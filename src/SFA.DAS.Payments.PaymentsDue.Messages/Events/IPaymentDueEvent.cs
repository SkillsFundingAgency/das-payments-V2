using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    public interface IPaymentDueEvent
    {
        long Ukprn { get; set; }

        Learner Learner { get; set; }

        LearningAim LearningAim { get; set; }

        string PriceEpisodeIdentifier { get; set; }

        decimal AmountDue { get; set; }

        CalendarPeriod CollectionPeriod { get; set; }

        CalendarPeriod DeliveryPeriod { get; set; }
    }
}