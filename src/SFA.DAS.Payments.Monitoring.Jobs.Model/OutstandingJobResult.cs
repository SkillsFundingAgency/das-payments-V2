using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Model
{
    public class OutstandingJobResult
    {
        public long? DcJobId { get; set; }
        public JobStatus JobStatus  { get; set; }
        public bool? DcJobSucceeded  { get; set; }
        public DateTimeOffset? StartTime  { get; set; }
        public DateTimeOffset? EndTime  { get; set; }
        public long? Ukprn { get; set; }
        public DateTime? IlrSubmissionTime { get; set; }

        public double JobRunTimeDurationInMillisecond => (DateTimeOffset.UtcNow.UtcDateTime - StartTime.GetValueOrDefault().UtcDateTime).TotalMilliseconds;
    }
}