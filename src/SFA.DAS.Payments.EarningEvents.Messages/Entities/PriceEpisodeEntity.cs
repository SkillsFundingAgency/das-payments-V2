using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SFA.DAS.Payments.EarningEvents.Messages.Entities
{
    [DataContract]
    public class PriceEpisodeEntity
    {
        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public IEnumerable<byte> Periods { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }
    }
}