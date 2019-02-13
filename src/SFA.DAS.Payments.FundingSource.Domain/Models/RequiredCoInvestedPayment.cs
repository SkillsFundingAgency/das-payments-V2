namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class RequiredPayment
    {
        public decimal SfaContributionPercentage { get; set; }
        public decimal AmountDue { get; set; }
    }

    public class RequiredCoInvestedPayment : RequiredPayment
    {
    }
}