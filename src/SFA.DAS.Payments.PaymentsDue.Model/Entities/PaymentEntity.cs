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
    }
}
