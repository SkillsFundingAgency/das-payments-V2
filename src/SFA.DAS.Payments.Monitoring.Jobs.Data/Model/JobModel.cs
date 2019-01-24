using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Model
{
    public class JobModel
    {
        public long Id { get; set; }
        public JobType JobType { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public JobStatus Status { get; set; }
        public long? DcJobId { get; set; }
        public long? Ukprn { get; set; }
        public DateTime? IlrSubmissionTime { get; set; }
        public int? LearnerCount { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
    }
}