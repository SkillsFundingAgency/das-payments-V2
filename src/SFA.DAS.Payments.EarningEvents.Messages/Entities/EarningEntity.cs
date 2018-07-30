using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.EarningEvents.Messages.Entities
{
    public class EarningEntity
    {
        public long Ukprn { get; set; }

        public string LearnRefNumber { get; set; }

        public ContractType ContractType { get; set; }

        public LearnerEntity Learner { get; set; }

        public LearningAimEntity LearnerAim { get; set; }

        public IEnumerable<PriceEpisodeEntity> PriceEpisodes { get; set; }
    }
}
