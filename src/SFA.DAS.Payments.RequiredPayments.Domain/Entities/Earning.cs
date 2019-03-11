namespace SFA.DAS.Payments.RequiredPayments.Domain.Entities
{
    public class Earning
    {
        public decimal Amount { get; set; }
        public EarningType EarningType { get; set; }
        public decimal? SfaContributionPercentage { get; set; }
    }
}
