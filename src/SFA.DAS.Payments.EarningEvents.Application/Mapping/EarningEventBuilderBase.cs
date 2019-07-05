using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class EarningEventBuilderBase
    {
        protected static List<IntermediateLearningAim> InitialLearnerTransform(ProcessLearnerCommand learnerSubmission, bool? mainAim)
        {
            var results = new List<IntermediateLearningAim>();

            foreach (var learningDelivery in learnerSubmission.Learner.LearningDeliveries)
            {
                if (mainAim.HasValue && mainAim.Value != learningDelivery.IsMainAim())
                    continue;

                var priceEpisodes = learnerSubmission.Learner.PriceEpisodes
                    .Where(x => x.PriceEpisodeValues.PriceEpisodeAimSeqNumber == learningDelivery.AimSeqNumber);
                var group = priceEpisodes.GroupBy(p => p.PriceEpisodeValues.PriceEpisodeContractType);

                foreach (var episodes in group)
                {
                    var intermediateAim = new IntermediateLearningAim(learnerSubmission, episodes, learningDelivery)
                    {
                        ContractType = GetContractType(episodes.Key)
                    };
                    results.Add(intermediateAim);
                }
                
            }

            return results;
        }

        private static ContractType GetContractType(string priceEpisodeContractType)
        {
            switch (priceEpisodeContractType)
            {
                case ApprenticeshipContractTypeEarningsEventFactory.Act1:
                    return ContractType.Act1;
                case ApprenticeshipContractTypeEarningsEventFactory.Act2:
                    return ContractType.Act2;
                default:
                    throw new InvalidOperationException($"Invalid contract type {priceEpisodeContractType}");
            }
        }
    }
}