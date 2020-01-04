namespace SFA.DAS.Payments.Monitoring.Metrics.Model.Submission
{
    public class DataLockedEarningsModel
    {
        public long Id { get; set; }
        public long SubmissionSummaryId { get; set; }
        public virtual SubmissionSummaryModel SubmissionSummary { get; set; }
        public DataLockTypeAmounts Amounts { get; set; }
    }
}