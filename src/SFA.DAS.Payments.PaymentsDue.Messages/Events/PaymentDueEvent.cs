using System;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    public abstract class PaymentDueEvent :PaymentsEvent, IPaymentDueEvent
    {
        public string PriceEpisodeIdentifier { get; set; }
        public decimal AmountDue { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }
        protected PaymentDueEvent()
        {
            EventTime = DateTimeOffset.UtcNow;
        }
    }
}
