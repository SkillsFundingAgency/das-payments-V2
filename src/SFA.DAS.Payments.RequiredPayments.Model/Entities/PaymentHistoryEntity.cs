using System;

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
        public string DeliveryPeriod { get; set; }
        public string CollectionPeriod { get; set; }
        public decimal Amount { get; set; }
    }
}
