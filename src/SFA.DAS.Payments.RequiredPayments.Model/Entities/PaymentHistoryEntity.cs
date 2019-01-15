using System;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Model.Entities
{
    public class PaymentHistoryEntity
    {
        public Guid ExternalId { get; set; }
        public long Ukprn { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public string LearnAimReference { get; set; }
        public int TransactionType { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public byte DeliveryPeriod { get; set; }
        public CollectionPeriod CollectionPeriod { get; set; }
        public decimal Amount { get; set; }
        public FundingSourceType FundingSource { get; set; }
    }
}
