namespace SFA.DAS.Payments.Monitoring.Metrics.Model.Submission
{
    public class SubmissionsSummaryModel
    {
        public long Id { get; set; }
        public bool IsWithinTolerance { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public long JobId { get; set; }
        public decimal Percentage { get; set; }
        public ContractTypeAmountsVerbose SubmissionMetrics { get; set; }
        public ContractTypeAmounts DcEarnings { get; set; }
        public ContractTypeAmountsVerbose DasEarnings { get; set; }
        public ContractTypeAmounts RequiredPayments { get; set; }
        public ContractTypeAmounts HeldBackCompletionPayments { get; set; }
        public decimal AdjustedDataLockedEarnings { get; set; }
        public decimal AlreadyPaidDataLockedEarnings { get; set; }
        public decimal TotalDataLockedEarnings { get; set; }
        public ContractTypeAmounts YearToDatePayments { get; set; }
    }
}