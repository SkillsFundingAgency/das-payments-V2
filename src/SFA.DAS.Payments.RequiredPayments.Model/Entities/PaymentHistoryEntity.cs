using System;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Model.Entities
{
    public class PaymentHistoryEntity
    {
        public Guid ExternalId { get; set; }
        public long Ukprn { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public string LearnAimReference { get; set; }
        public string LearningAimFundingLineType { get; set; }
        public int TransactionType { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public byte DeliveryPeriod { get; set; }
        public CollectionPeriod CollectionPeriod { get; set; }
        public decimal Amount { get; set; }
        public FundingSourceType FundingSource { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public long? AccountId { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public ContractType ContractType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public byte CompletionStatus { get; set; }
        public decimal CompletionAmount { get; set; }
        public decimal InstalmentAmount { get; set; }
        public short NumberOfInstalments { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public string ReportingAimFundingLineType { get; set; }
    }
}
