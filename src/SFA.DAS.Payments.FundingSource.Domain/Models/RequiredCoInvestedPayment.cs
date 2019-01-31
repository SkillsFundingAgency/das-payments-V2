namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public abstract class RequiredPayment
    {
        public decimal SfaContributionPercentage { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountFunded { get; set; }
    }

    public class RequiredLevyPayment : RequiredPayment
    {
        public decimal LevyBalance { get; set; }
    }

    public class RequiredCoInvestedPayment : RequiredPayment
    {
    }
}