using System.Collections.Generic;
using SFA.DAS.Payments.EarningEvents.Messages.Entities;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public interface IEarningEvent : IPaymentsEvent
    {
        long Ukprn { get; set; }

        string LearnRefNumber { get; set; }

        ContractType ContractType { get; set; }

        LearnerEntity Learner { get; set; }

        string LearnAimRef { get; set; }

        IEnumerable<PriceEpisodeEntity> PriceEpisodes { get; set; }
    }
}
