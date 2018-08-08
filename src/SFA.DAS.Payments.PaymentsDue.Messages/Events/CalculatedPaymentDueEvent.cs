using System;
using SFA.DAS.Payments.PaymentsDue.Messages.Entities;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    public class CalculatedPaymentDueEvent : ICalculatedPaymentDueEvent
    {
        public DateTimeOffset EventTime { get; }
        public PaymentDueEntity PaymentDueEntity { get; set; }
    }
}