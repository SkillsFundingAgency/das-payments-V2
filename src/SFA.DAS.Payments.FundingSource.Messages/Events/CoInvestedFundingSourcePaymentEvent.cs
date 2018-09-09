namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public class CoInvestedFundingSourcePaymentEvent : FundingSourcePaymentEvent
    {
        public short ContractType { get; set; }

        public decimal SfaContributionPercentage { get; set; }
    }
}