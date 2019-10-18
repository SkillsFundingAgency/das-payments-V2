using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System;

namespace SFA.DAS.Payments.Audit.Model
{
    public interface IPeriodisedPaymentsEventModel : IPaymentsEventModel
    {
        Guid EarningEventId { get; set; }
        string PriceEpisodeIdentifier { get; set; }
        ContractType ContractType { get; set; }
        TransactionType TransactionType { get; set; }
        decimal Amount { get; set; }
        byte DeliveryPeriod { get; set; }
        decimal SfaContributionPercentage { get; set; }
        string AgreementId { get; set; }
        long? AccountId { get; set; }
        long? TransferSenderAccountId { get; set; }
        long? ApprenticeshipId { get; set; }
        long? ApprenticeshipPriceEpisodeId { get; set; }
        ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        UnpaidReason? UnpaidReason { get; set; }
    }

    public abstract class PeriodisedPaymentsEventModel : PaymentsEventModel, IPeriodisedPaymentsEventModel
    {
        public Guid EarningEventId { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public ContractType ContractType { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public byte DeliveryPeriod { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public string AgreementId { get; set; }
        public long? AccountId { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public long? ApprenticeshipId { get; set; }
        public long? ApprenticeshipPriceEpisodeId { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public UnpaidReason? UnpaidReason { get; set; }
    }
}