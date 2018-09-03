using System;

namespace SFA.DAS.Payments.Messages.Core
{
    public class RecordablePaymentEvent : IRecordablePaymentEvent
    {
        public DateTimeOffset EventTime { get; set; }

        public string JobId { get; set; }
    }
}