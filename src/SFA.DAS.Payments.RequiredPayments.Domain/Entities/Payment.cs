using System;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public FundingSourceType FundingSource { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public byte DeliveryPeriod { get; set; }

        public CollectionPeriod CollectionPeriod { get; set; }
        public string LearnAimReference { get; set; }
        public int TransactionType { get; set; }

        public decimal SfaContributionPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public byte CompletionStatus { get; set; }
        public decimal CompletionAmount { get; set; }
        public decimal InstalmentAmount { get; set; }
        public short NumberOfInstalments { get; set; }
        public long? AccountId { get; set; }
        public long? TransferSenderAccountId { get; set; }
    }
}
