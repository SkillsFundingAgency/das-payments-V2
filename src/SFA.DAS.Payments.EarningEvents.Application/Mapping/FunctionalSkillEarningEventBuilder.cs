using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                var contractTypes = intermediateLearningAim.Learner.LearningDeliveries.GetContractTypesForLearningDeliveries();
                var distinctContractTypes = contractTypes.Distinct().ToList();
                foreach (var distinctContractType in distinctContractTypes)
                {
                    var functionalSkillEarning = mapper.Map<FunctionalSkillEarningsEvent>(intermediateLearningAim);
                    functionalSkillEarning.ContractType = distinctContractType;
                    foreach (var earning in functionalSkillEarning.Earnings)
                    {
                        earning.Periods = GetEarningPeriodsMatchingContractType(contractTypes, distinctContractType, earning.Periods.ToList());
                    }
                    if (functionalSkillEarning.Earnings.All(earning => earning.Periods.Any()))
                        results.Add(functionalSkillEarning);
                }
            }

            return results;
        }

        private ReadOnlyCollection<EarningPeriod> GetEarningPeriodsMatchingContractType(ContractType[] contractTypes,
            ContractType learningDeliveryPeriodContractType,
            List<EarningPeriod> earningPeriods)
        {
            const byte periods = 12;

            var outputEarnings = new List<EarningPeriod>();
            for (byte i = 1; i <= periods; i++)
            {
                if (contractTypes[i - 1] != learningDeliveryPeriodContractType) continue;
                var earningPeriod = earningPeriods.SingleOrDefault(m => m.Period == i);
                if (earningPeriod != null)
                    outputEarnings.Add(earningPeriod);
            }
            return outputEarnings.AsReadOnly();
        }

        private static readonly Dictionary<string, FunctionalSkillType> TypeMap =
            new Dictionary<string, FunctionalSkillType>
            {
                {"MathEngBalPayment", FunctionalSkillType.BalancingMathsAndEnglish},
                {"MathEngOnProgPayment", FunctionalSkillType.OnProgrammeMathsAndEnglish},
                {"PriceEpisodeLSFCash", FunctionalSkillType.LearningSupport},
            };


        private static Dictionary<FunctionalSkillType, string> MathsAndEnglishAttributes()
        {
            return new Dictionary<FunctionalSkillType, string>
            {
                {FunctionalSkillType.OnProgrammeMathsAndEnglish, "MathEngOnProgPayment"},
                {FunctionalSkillType.BalancingMathsAndEnglish, "MathEngBalPayment"},
                {FunctionalSkillType.LearningSupport, "PriceEpisodeLSFCash" },
            };
        }

    }
}