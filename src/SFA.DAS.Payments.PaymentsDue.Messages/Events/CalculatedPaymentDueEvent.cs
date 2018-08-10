using System;
using System.Runtime.Serialization;
using SFA.DAS.Payments.PaymentsDue.Messages.Entities;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    [DataContract]
    public class CalculatedPaymentDueEvent : ICalculatedPaymentDueEvent
    {
        [DataMember]
        public DateTimeOffset EventTime { get; set; }

        [DataMember]
        public PaymentDueEntity PaymentDueEntity { get; set; }
    }
}