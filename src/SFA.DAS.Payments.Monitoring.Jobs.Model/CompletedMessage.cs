using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Model
{
    public class CompletedMessage
    {
        public long JobId { get; set; }
        public Guid MessageId { get; set; }
        public bool Succeeded { get; set; }
        public DateTimeOffset CompletedTime { get; set; }
    }
}