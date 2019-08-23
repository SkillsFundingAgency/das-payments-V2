using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Model
{
    public class JobMessageStartedModel
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public Guid MessageId { get; set; }
        public Guid? ParentMessageId { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public string MessageName { get; set; }
        public virtual JobModel Job { get; set; }
    }
}