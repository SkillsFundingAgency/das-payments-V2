namespace SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd
{
    public interface IPeriodEndSummaryModel
    {
        ContractTypeAmounts DcEarnings { get; set; }
        ContractTypeAmounts Payments { get; set; }
        decimal AdjustedDataLockedEarnings { get; set; }
        ContractTypeAmounts HeldBackCompletionPayments { get; set; }
        ContractTypeAmounts YearToDatePayments { get; set; }
    }
}