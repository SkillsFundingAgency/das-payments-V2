using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Model
{
    public abstract class PeriodisedPaymentsEventModel: PaymentsEventModel
    {
        //public long Id { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public ContractType ContractType { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public byte DeliveryPeriod { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public string AgreementId { get; set; }
    }
}