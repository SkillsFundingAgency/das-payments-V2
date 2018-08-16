using System;
using System.Runtime.Serialization;
using SFA.DAS.Payments.RequiredPayments.Messages.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
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