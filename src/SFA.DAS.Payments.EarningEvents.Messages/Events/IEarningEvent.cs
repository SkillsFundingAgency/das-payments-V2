using System.Collections.Generic;
using SFA.DAS.Payments.EarningEvents.Messages.Entities;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public interface IEarningEvent : IPaymentsEvent
    {
        long Ukprn { get; }

        string LearnRefNumber { get; }

        ContractType ContractType { get; }

        LearnerEntity Learner { get; }

        LearnAimEntity LearnAim { get; }

        IEnumerable<PriceEpisodeEntity> PriceEpisodes { get; }
    }
}
