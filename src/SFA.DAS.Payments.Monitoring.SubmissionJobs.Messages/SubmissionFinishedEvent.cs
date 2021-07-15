using System;

namespace SFA.DAS.Payments.Monitoring.SubmissionJobs.Messages
{
    public abstract class SubmissionFinishedEvent
    {
        public Guid EventId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long JobId { get; set; }
        public long Ukprn { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }

        protected SubmissionFinishedEvent()
        {
            EventTime = DateTimeOffset.UtcNow;
            EventId = Guid.NewGuid();
        }
    }
}