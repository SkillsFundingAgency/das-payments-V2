namespace SFA.DAS.Payments.Monitoring.Metrics.Model.Submission
{
    public class DataLockCountsModel
    {
        public long Id { get; set; }
        public long SubmissionSummaryId { get; set; }
        public virtual SubmissionSummaryModel SubmissionSummary { get; set; }
        public DataLockTypeCounts Amounts { get; set; } = new DataLockTypeCounts();
    }
}