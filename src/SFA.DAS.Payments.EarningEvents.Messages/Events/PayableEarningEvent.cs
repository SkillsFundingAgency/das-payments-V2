using System;
using System.Collections.Generic;
using SFA.DAS.Payments.EarningEvents.Messages.Entities;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class PayableEarningEvent : IPayableEarningEvent
    {
        public DateTimeOffset EventTime { get; set; }
        public long Ukprn { get; set; }
        public string LearnRefNumber { get; set; }
        public ContractType ContractType { get; set; }
        public LearnerEntity Learner { get; set; }
        public LearnAimEntity LearnAim { get; set; }
        public IEnumerable<PriceEpisodeEntity> PriceEpisodes { get; set; }
    }
}