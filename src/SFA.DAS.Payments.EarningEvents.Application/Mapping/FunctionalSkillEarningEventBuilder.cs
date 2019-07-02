using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class FunctionalSkillEarningEventBuilder : EarningEventBuilderBase, IFunctionalSkillEarningsEventBuilder
    {
        private readonly IMapper mapper;

        public FunctionalSkillEarningEventBuilder(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public List<FunctionalSkillEarningsEvent> Build(ProcessLearnerCommand learnerSubmission)
        {
            var intermediateResults = InitialLearnerTransform(learnerSubmission, false);
            var results = new List<FunctionalSkillEarningsEvent>();

            foreach (var intermediateLearningAim in intermediateResults)
            {
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

                var functionalSkillEarning =
                    mapper.Map<FunctionalSkillEarningsEvent>(learnerWithSortedPriceEpisodes);

                functionalSkillEarning.ContractType = intermediateLearningAim.ContractType;

                var currentEarnings = new List<FunctionalSkillEarning>(functionalSkillEarning.Earnings);

                foreach (var mathsAndEnglishAttribute in MathsAndEnglishAttributes())
                {
                    var matchingEarning =
                        currentEarnings.SingleOrDefault(x => x.Type == mathsAndEnglishAttribute.Key);

                    if (matchingEarning == null)
                    {
                        continue;
                    }

                    var matchingPeriods = new List<EarningPeriod>(matchingEarning.Periods);

                    var outputEarnings = new List<EarningPeriod>();

                    for (byte i = 1; i <= earningPeriods; i++)
                    {
                        if (contractTypes[i - 1] == functionalSkillEarning.ContractType)
                        {
                            outputEarnings.Add(matchingPeriods.SingleOrDefault(m => m.Period == i));
                        }
                    }

                    matchingEarning.Periods = outputEarnings.AsReadOnly();
                }

                results.Add(functionalSkillEarning);
            }

            return results;
        }

        private static ContractType GetContractType(string contractType)
        {
            switch (contractType)
            {
                case "Levy Contract":
                    return ContractType.Act1;
                case "Non-Levy Contract":
                    return ContractType.Act2;
                default:
                    throw new InvalidOperationException("unable to determine correct contract type");
            }
        }

        private static Dictionary<FunctionalSkillType, string> MathsAndEnglishAttributes()
        {
            return new Dictionary<FunctionalSkillType, string>
            {
                {FunctionalSkillType.OnProgrammeMathsAndEnglish, "MathEndOnProgPayment"},
                {FunctionalSkillType.BalancingMathsAndEnglish, "MathEngBalPayment"}
            };
        }
    }
}