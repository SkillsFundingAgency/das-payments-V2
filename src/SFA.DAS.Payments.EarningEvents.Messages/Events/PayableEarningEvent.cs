using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SFA.DAS.Payments.EarningEvents.Messages.Entities;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    [DataContract]
    public class PayableEarningEvent : IPayableEarningEvent
    {
        [DataMember]
        public DateTimeOffset EventTime { get; set; }

        [DataMember]
        public long Ukprn { get; set; }

        [DataMember]
        public string LearnRefNumber { get; set; }

        [DataMember]
        public ContractType ContractType { get; set; }

        [DataMember]
        public LearnerEntity Learner { get; set; }

        [DataMember]
        public LearnAimEntity LearnAim { get; set; }

        [DataMember]
        public IEnumerable<PriceEpisodeEntity> PriceEpisodes { get; set; }

        [DataMember]
        public string JobId { get; set; }
    }
}