using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Model
{
    public interface IPeriodisedPaymentsEventModel: IPaymentsEventModel
    {
         string PriceEpisodeIdentifier { get; set; }
         ContractType ContractType { get; set; }
         TransactionType TransactionType { get; set; }
         decimal Amount { get; set; }
         byte DeliveryPeriod { get; set; }
         decimal SfaContributionPercentage { get; set; }
         string AgreementId { get; set; }
         long? AccountId { get; set; }
         long? TransferSenderAccountId { get; set; }
    }

    public abstract class PeriodisedPaymentsEventModel: PaymentsEventModel, IPeriodisedPaymentsEventModel
    {
        //public long Id { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public ContractType ContractType { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public byte DeliveryPeriod { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public string AgreementId { get; set; }
        public long? AccountId { get; set; }
        public long? TransferSenderAccountId { get; set; }
    }
}