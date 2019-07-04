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
                const byte earningPeriods = 12;

                var contractTypes = intermediateLearningAim.Learner.LearningDeliveries.GetContractTypesForLearningDeliveries();

                var learnerWithSortedPriceEpisodes =
                    intermediateLearningAim.CopyReplacingPriceEpisodes(intermediateLearningAim.PriceEpisodes);

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