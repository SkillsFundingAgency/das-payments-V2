using System;
using System.Runtime.Serialization;

namespace SFA.DAS.Payments.RequiredPayments.Model.Entities
{
    [DataContract]
    public class PaymentEntity
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public long Ukprn { get; set; }
        [DataMember]
        public string LearnerReferenceNumber { get; set; }
        [DataMember]
        public string ApprenticeshipKey { get; set; }
        [DataMember]
        public string PriceEpisodeIdentifier { get; set; }
        [DataMember]
        public string DeliveryPeriod { get; set; }
        [DataMember]
        public string CollectionPeriod { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
    }
}
