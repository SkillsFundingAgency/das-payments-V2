using SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data.Entities;

namespace SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data
{
    public class SubmissionData
    {
        public JobModel JobModel { get; set; }

        public long EarningEvent { get; set; }
        public long EarningEventPeriod { get; set; }
        public long EarningEventPriceEpisode { get; set; }

        public long FundingSourceEvent { get; set; }

        public long RequiredPaymentEvent { get; set; }

        public long DataLockEvent { get; set; }
        public long DataLockEventNonPayablePeriod { get; set; }
        public long DataLockEventNonPayablePeriodFailures { get; set; }
        public long DataLockEventPriceEpisode { get; set; }
        public long DataLockPayablePeriod { get; set; }
        public string SubmissionId { get; set; }
        public byte CollectionPeriod { get; set; }
    }
}