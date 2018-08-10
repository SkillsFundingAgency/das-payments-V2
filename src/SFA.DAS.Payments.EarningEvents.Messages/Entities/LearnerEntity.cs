using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SFA.DAS.Payments.EarningEvents.Messages.Entities
{
    [DataContract]
    public class LearnerEntity
    {
        public long Ukprn { get; set; }

        public string LearnerReferenceNumber { get; set; }

        public long Uln { get; set; }
    }
}
