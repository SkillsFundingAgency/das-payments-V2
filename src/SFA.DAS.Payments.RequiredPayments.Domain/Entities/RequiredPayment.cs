namespace SFA.DAS.Payments.RequiredPayments.Domain.Entities
{
    public class RequiredPayment
    {
        public decimal Amount { get; set; }
        public EarningType EarningType { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public long? AccountId { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public int TransactionType { get; set; }

    }
}
