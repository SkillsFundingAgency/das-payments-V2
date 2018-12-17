using System;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Model
{
    public class ProviderEarningsJobModel
    {
        public long Id { get; set; }
        public long DcJobId { get; set; }
        public long Ukprn { get; set; }
        public DateTime IlrSubmissionTime { get; set; }
        public short CollectionYear { get; set; }
        public byte CollectionPeriod { get; set; }

        public virtual JobModel Job { get; set; }
    }
}