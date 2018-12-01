using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public abstract class PeriodisedPaymentEvent : PaymentsEvent, IPeriodisedPaymentEvent
    {
        public string PriceEpisodeIdentifier { get; set; }
        public decimal AmountDue { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }
    }
}