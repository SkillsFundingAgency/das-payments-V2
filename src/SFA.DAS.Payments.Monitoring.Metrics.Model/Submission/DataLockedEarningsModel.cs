namespace SFA.DAS.Payments.Monitoring.Metrics.Model.Submission
{
    public class DataLockedEarningsModel
    {
        public SubmissionSummaryModel SubmissionSummary { get; set; }
        public DataLockTypeAmounts Amounts { get; set; }
    }
}