using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SFA.DAS.Payments.PaymentsDue.Model.Entities
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
    }
}
