using System;
using System.Collections.Generic;
using System.Globalization;
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
                    .Where(x => x.PriceEpisodeValues.PriceEpisodeAimSeqNumber == learningDelivery.AimSeqNumber)
                    .ToList();

                if (!learningDelivery.IsMainAim() && LearningDeliveryHasContract(learningDelivery))
                {
                    // Maths & English
                    var mathsAndEnglishAims = GetMathsAndEnglishAim(learnerSubmission, learningDelivery, mainAim.HasValue);
                    results.AddRange(mathsAndEnglishAims);

                    continue;
                }

                var group = priceEpisodes.Where(pe => IsCurrentAcademicYear(pe.PriceEpisodeValues, learnerSubmission.CollectionYear))
                    .GroupBy(p => p.PriceEpisodeValues.PriceEpisodeContractType);

                foreach (var episodes in group)
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

        private static bool LearningDeliveryHasContract(LearningDelivery learningDelivery)
        {
            var periodisedTextValues = learningDelivery.LearningDeliveryPeriodisedTextValues
                .Where(l => l.AttributeName == "LearnDelContType").ToList();

            const byte earningPeriods = 12;
            var periodFundingLineTypes = new List<string>();
            for (byte i = 1; i <= earningPeriods; i++)
            {
                periodFundingLineTypes.AddRange(periodisedTextValues.Select(p => p.GetPeriodTextValue(i)));
            }

            return periodFundingLineTypes.Any(x => !x.Equals("none", StringComparison.InvariantCultureIgnoreCase));
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
                    .SelectMany(o => o)
                    .Where(ct => ct != ContractType.None);

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