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
         DateTime StartDate { get; set; }
         DateTime PlannedEndDate { get; set; }
         DateTime? ActualEndDate { get; set; }
         byte CompletionStatus { get; set; }
         decimal CompletionAmount { get; set; }
         decimal InstalmentAmount { get; set; }
         short NumberOfInstalments { get; set; }
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
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public byte CompletionStatus { get; set; }
        public decimal CompletionAmount { get; set; }
        public decimal InstalmentAmount { get; set; }
        public short NumberOfInstalments { get; set; }
    }
}