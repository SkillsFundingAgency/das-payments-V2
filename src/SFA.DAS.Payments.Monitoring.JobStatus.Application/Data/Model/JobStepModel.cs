using System;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Model
{
    public class JobStepModel
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public Guid MessageId { get; set; }
        public Guid? ParentMessageId { get; set; }
        public JobEventStatus Status { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }    

        public virtual JobModel Job { get; set; }
    }
}