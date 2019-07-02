using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
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

                if (!priceEpisodes.Any())
                {
                    // Maths & English
                    var mathsAndEnglishAims = GetMathsAndEnglishAim(learnerSubmission, learningDelivery);
                    results.AddRange(mathsAndEnglishAims);

                    continue;
                }

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

        private static List<IntermediateLearningAim> GetMathsAndEnglishAim(ProcessLearnerCommand learnerSubmission,LearningDelivery learningDelivery)
        {
            var results = new List<IntermediateLearningAim>();

            var intermediateLearningAim =
                new IntermediateLearningAim(learnerSubmission, new List<PriceEpisode>(), learningDelivery);

            var learningDeliveries = intermediateLearningAim.Learner.LearningDeliveries;

            var periodisedTextValues = learningDeliveries.Where(x => x.LearningDeliveryPeriodisedTextValues != null)
                .SelectMany(x =>
                    x.LearningDeliveryPeriodisedTextValues.Where(l => l.AttributeName == "LearnDelContType"));

            const byte earningPeriods = 12;

            var learnerWithSortedPriceEpisodes =
                intermediateLearningAim.CopyReplacingPriceEpisodes(intermediateLearningAim.PriceEpisodes);

            var contractTypes = new ContractType[earningPeriods];

            for (byte i = 1; i <= earningPeriods; i++)
            {
                var periodValues = periodisedTextValues.Select(p => p.GetPeriodTextValue(i)).ToArray();
                var periodValue = GetContractType(periodValues[0]);

                contractTypes[i - 1] = periodValue;
            }

            var distinctContractTypes = contractTypes.Distinct().ToList();

            distinctContractTypes.ForEach(c =>
            {
                var mathsAndEnglishAim =
                    new IntermediateLearningAim(learnerSubmission, new List<PriceEpisode>(), learningDelivery)
                    {
                        ContractType = c
                    };

                results.Add(mathsAndEnglishAim);
            });

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