using System;

namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public class RecordablePaymentEvent : IRecordablePaymentEvent
    {
        public DateTimeOffset EventTime { get; set; }

        public string JobId { get; set; }
    }
}