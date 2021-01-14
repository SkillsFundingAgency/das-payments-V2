using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class EarningEventBuilderBase
    {
        protected static List<IntermediateLearningAim> InitialLearnerTransform(ProcessLearnerCommand learnerSubmission, bool? mainAim)
        {
            var results = new List<IntermediateLearningAim>();
            var groupedLearningDeliveries = learnerSubmission.Learner.LearningDeliveries
                .GroupBy(ld => new
                {
                    ld.LearningDeliveryValues.LearnAimRef,
                    ld.LearningDeliveryValues.FworkCode,
                    ld.LearningDeliveryValues.ProgType,
                    ld.LearningDeliveryValues.PwayCode,
                    ld.LearningDeliveryValues.StdCode
                });

            foreach (var groupedLearningDelivery in groupedLearningDeliveries)
            {
                var orderedGroupedLearningDelivery = groupedLearningDelivery.OrderByDescending(x => x.LearningDeliveryValues.LearnStartDate).ToList();
                var learningDelivery = orderedGroupedLearningDelivery.First();
                if (mainAim.HasValue && mainAim.Value != learningDelivery.IsMainAim())
                    continue;


                var priceEpisodes = learnerSubmission.Learner.PriceEpisodes
                    .Where(x => orderedGroupedLearningDelivery.Any(g => g.AimSeqNumber == x.PriceEpisodeValues.PriceEpisodeAimSeqNumber))
                    .ToList();

                if (!learningDelivery.IsMainAim())
                {
                    // Maths & English
                    var mathsAndEnglishAims = GetMathsAndEnglishAim(learnerSubmission, orderedGroupedLearningDelivery, mainAim.HasValue);
                    results.AddRange(mathsAndEnglishAims);

                    continue;
                }

                var group = priceEpisodes.Where(pe => IsCurrentAcademicYear(pe.PriceEpisodeValues, learnerSubmission.CollectionYear))
                    .GroupBy(p => p.PriceEpisodeValues.PriceEpisodeContractType);

                foreach (var episodes in group)
                {
                    var intermediateAim = new IntermediateLearningAim(learnerSubmission, episodes, orderedGroupedLearningDelivery)
                    {
                        ContractType = MappingExtensions.GetContractType(episodes.Key)
                    };
                    results.Add(intermediateAim);
                }

            }

            return results;
        }


        private static bool IsCurrentAcademicYear(PriceEpisodeValues priceEpisodeValues, short currentAcademicYear)
        {
            var calendarYear = currentAcademicYear / 100 + 2000;
            var yearStartDate = new DateTime(calendarYear, 8, 1);
            var yearEndDate = yearStartDate.AddYears(1);

            var episodeStartDate = priceEpisodeValues.EpisodeStartDate;
            return episodeStartDate.HasValue &&
                   episodeStartDate.Value >= yearStartDate &&
                   episodeStartDate.Value < yearEndDate;
        }

        private static List<IntermediateLearningAim> GetMathsAndEnglishAim(ProcessLearnerCommand learnerSubmission,
            List<LearningDelivery> learningDeliveries, bool singleContractType)
        {
            if (singleContractType)
                return new List<IntermediateLearningAim> { new IntermediateLearningAim(learnerSubmission, new List<PriceEpisode>(), learningDeliveries) };

            var results = new List<IntermediateLearningAim>();
            var contractTypes = learningDeliveries.GetContractTypesForLearningDeliveries();

            var distinctContractTypes = contractTypes.Distinct().ToList();

            distinctContractTypes.ForEach(c =>
            {
                var mathsAndEnglishAim =
                    new IntermediateLearningAim(learnerSubmission, new List<PriceEpisode>(), learningDeliveries)
                    {
                        ContractType = c
                    };

                results.Add(mathsAndEnglishAim);
            });

            return results;
        }

        public List<byte> CalculateRedundancyPeriods(List<PriceEpisode> priceEpisodes)
        {
            if (priceEpisodes.All(x => x.PriceEpisodeValues.PriceEpisodeRedStatusCode != 1))
                return new List<byte>();

            var redundancyPeriods = new List<byte>();

            var orderedPriceEpisodes = priceEpisodes.OrderBy(x => x.PriceEpisodeValues.EpisodeStartDate).ToList();
            var priceEpisodeCount = orderedPriceEpisodes.Count;

            for (var i = 0; i < priceEpisodeCount; i++)
            {
                var currentPriceEpisode = orderedPriceEpisodes[i];
                if (currentPriceEpisode.PriceEpisodeValues.PriceEpisodeRedStatusCode == 1)
                {
                    var redundancyStartPeriod = currentPriceEpisode.PriceEpisodeValues.PriceEpisodeRedStartDate.Value.GetPeriodFromDate();
                    var redundancyEndPeriod = 13;
                    if (i + 1 < priceEpisodeCount)
                    {
                        redundancyEndPeriod = orderedPriceEpisodes[i + 1].PriceEpisodeValues.EpisodeStartDate.Value.GetPeriodFromDate();
                    }

                    for (byte j = redundancyStartPeriod; j < redundancyEndPeriod; j++)
                    {
                        redundancyPeriods.Add(j);
                    }
                }
            }

            return redundancyPeriods;
        }
    }
}