namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class CoInvestedPayment
    {
        public decimal SfaContributionPercentage { get; set; }
        public decimal AmountDue { get; set; }
    }
}