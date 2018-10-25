using System;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public abstract class RequiredPaymentEvent : PaymentsEvent,IRequiredPayment
    {
        public string PriceEpisodeIdentifier { get; set;}
        public decimal AmountDue { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }

    }
}