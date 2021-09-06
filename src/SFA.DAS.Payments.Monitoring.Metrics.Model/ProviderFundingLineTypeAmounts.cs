namespace SFA.DAS.Payments.Monitoring.Metrics.Model
{
    public class ProviderFundingLineTypeAmounts
    {
        public long Ukprn { get; set; }
        public decimal FundingLineType16To18Amount { get; set; }
        public decimal FundingLineType19PlusAmount { get; set; }
        public decimal Total => FundingLineType16To18Amount + FundingLineType19PlusAmount;
    }
}