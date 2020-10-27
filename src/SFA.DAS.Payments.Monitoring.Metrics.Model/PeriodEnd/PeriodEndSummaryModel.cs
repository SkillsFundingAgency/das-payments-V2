namespace SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd
{
    public class PeriodEndSummaryModel : IPeriodEndSummaryModel
    {
        public long Id { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public long JobId { get; set; }
        public decimal Percentage { get; set; }
        public ContractTypeAmountsVerbose PaymentMetrics { get; set; } = new ContractTypeAmountsVerbose();
        public ContractTypeAmounts DcEarnings { get; set; } = new ContractTypeAmounts();
        public ContractTypeAmounts Payments { get; set; } = new ContractTypeAmounts();
        public decimal AdjustedDataLockedEarnings { get; set; }
        public decimal AlreadyPaidDataLockedEarnings { get; set; }
        public decimal TotalDataLockedEarnings { get; set; }
        public ContractTypeAmounts HeldBackCompletionPayments { get; set; } = new ContractTypeAmounts();
        public ContractTypeAmounts YearToDatePayments { get; set; } = new ContractTypeAmounts();
    }
}