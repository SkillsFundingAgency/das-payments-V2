using SFA.DAS.Payments.Model.Core.Entities;

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
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public long? ApprenticeshipId { get; set; }
        public long? ApprenticeshipPriceEpisodeId { get; set; }
    }
}
