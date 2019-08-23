using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Model
{
    public class JobMessageFinishedModel
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public Guid MessageId { get; set; }
        public JobMessageStatus Status { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string MessageName { get; set; }
        public virtual JobModel Job { get; set; }
    }
}