using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Model
{
    public class JobModel
    {
        public long Id { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public JobStatus Status { get; set; }

        //public virtual List<ProviderEarningsJobModel> ProviderEarnings { get; set; }
        //public virtual List<JobStepModel> JobEvents { get; set; }
    }
}