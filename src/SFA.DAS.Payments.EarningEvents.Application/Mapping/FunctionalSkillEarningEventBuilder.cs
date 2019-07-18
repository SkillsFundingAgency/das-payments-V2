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

        public List<ApprenticeshipContract2TypeFunctionalSkillEarningsEvent> Build(ProcessLearnerCommand learnerSubmission)
        {
            var intermediateResults = InitialLearnerTransform(learnerSubmission, false);
            var results = new List<ApprenticeshipContract2TypeFunctionalSkillEarningsEvent>();

            foreach (var intermediateLearningAim in intermediateResults)
            {
                var contractTypes = intermediateLearningAim.Learner.LearningDeliveries.GetContractTypesForLearningDeliveries();
                var distinctContractTypes = contractTypes.Distinct().ToList();

                var learnerWithSortedPriceEpisodes = intermediateLearningAim.CopyReplacingPriceEpisodes(intermediateLearningAim.PriceEpisodes);
                var functionalSkillEarning = mapper.Map<ApprenticeshipContract2TypeFunctionalSkillEarningsEvent>(learnerWithSortedPriceEpisodes);

                foreach (var contractType in distinctContractTypes)
                {
                    var currentEarnings = new List<FunctionalSkillEarning>(functionalSkillEarning.Earnings);
                    foreach (var earning in functionalSkillEarning.Earnings)
                    {
                        earning.Periods =  GetEarningPeriodsMatchingContractType(contractTypes, contractType, earning.Periods.ToList());
                    }

                    functionalSkillEarning.ContractType = intermediateLearningAim.ContractType;
                }

                results.Add(functionalSkillEarning);
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
                if (contractTypes[i - 1] == learningDeliveryPeriodContractType)
                {
                    outputEarnings.Add(earningPeriods.SingleOrDefault(m => m.Period == i));
                }
            }
            return  outputEarnings.AsReadOnly();
        }
      
    }
}