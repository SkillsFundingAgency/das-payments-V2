using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class EarningEventBuilderBase
    {
        protected static List<IntermediateLearningAim> InitialLearnerTransform(ProcessLearnerCommand learnerSubmission, bool? mainAim)
        {
            var results = new List<IntermediateLearningAim>();
            var groupedLearningDeliveries = learnerSubmission.Learner.LearningDeliveries
                .Where(x => x.IsMainAim() || x.LearningDeliveryValues.LearnDelMathEng.HasValue && x.LearningDeliveryValues.LearnDelMathEng.Value)
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
                var learningDelivery = groupedLearningDelivery.First(); // Potential loss of data!!!
                if (mainAim.HasValue && mainAim.Value != learningDelivery.IsMainAim())
                    continue;

                var priceEpisodes = learnerSubmission.Learner.PriceEpisodes
                    .Where(x => groupedLearningDelivery.Any(g => g.AimSeqNumber == x.PriceEpisodeValues.PriceEpisodeAimSeqNumber))
                    .ToList();

                if (!learningDelivery.IsMainAim())
                {
                    // Maths & English
                    var mathsAndEnglishAims = GetMathsAndEnglishAim(learnerSubmission, learningDelivery, mainAim.HasValue);
                    results.AddRange(mathsAndEnglishAims);

                    continue;
                }

                var group = priceEpisodes.Where(pe => IsCurrentAcademicYear(pe.PriceEpisodeValues, learnerSubmission.CollectionYear))
                    .GroupBy(p => p.PriceEpisodeValues.PriceEpisodeContractType);

                foreach (var episodes in @group)
                {
                    var intermediateAim = new IntermediateLearningAim(learnerSubmission, episodes, learningDelivery)
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
            LearningDelivery learningDelivery, bool singleContractType)
        {
            if (singleContractType)
                return new List<IntermediateLearningAim> { new IntermediateLearningAim(learnerSubmission, new List<PriceEpisode>(), learningDelivery) };

            var results = new List<IntermediateLearningAim>();

            var intermediateLearningAim =
                new IntermediateLearningAim(learnerSubmission, new List<PriceEpisode>(), learningDelivery);

            var contractTypes =
                intermediateLearningAim.Learner
                    .LearningDeliveries
                    .Select(x => x.GetContractTypesForLearningDeliveries())
                    .SelectMany(o => o);

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
    }
}