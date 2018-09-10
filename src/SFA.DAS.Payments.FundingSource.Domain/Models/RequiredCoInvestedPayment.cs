namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class RequiredCoInvestedPayment
    {
        public decimal SfaContributionPercentage { get; set; }
        public decimal AmountDue { get; set; }
    }
}