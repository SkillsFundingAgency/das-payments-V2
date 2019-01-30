namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public abstract class RequiredPaymentBase
    {
        public decimal SfaContributionPercentage { get; set; }
        public decimal AmountDue { get; set; }
    }

    public class RequiredLevyPayment : RequiredPaymentBase
    {
    }

    public class RequiredCoInvestedPayment : RequiredPaymentBase
    {
    }
}