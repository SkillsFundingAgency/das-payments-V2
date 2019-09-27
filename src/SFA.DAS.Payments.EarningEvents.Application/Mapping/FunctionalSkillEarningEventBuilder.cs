using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

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
                var contractTypes = new List<LearningDelivery>{ intermediateLearningAim.Aim }.GetContractTypesForLearningDeliveries();
                var distinctContractTypes = contractTypes.Distinct().ToList();

                var learnerWithSortedPriceEpisodes = intermediateLearningAim.CopyReplacingPriceEpisodes(intermediateLearningAim.PriceEpisodes);
                
                foreach (var contractType in distinctContractTypes)
                {
                    var functionalSkillEarning =
                        GetContractTypeFunctionalSkillEarningEvent(learnerWithSortedPriceEpisodes, contractType);

                    foreach (var earning in functionalSkillEarning.Earnings)
                    {
                        earning.Periods = GetEarningPeriodsMatchingContractType(contractTypes, 
                            contractType, earning.Periods.ToList());
                    }

                    results.Add(functionalSkillEarning);
                }
            }

            return results.Distinct().ToList();
        }

        private FunctionalSkillEarningsEvent GetContractTypeFunctionalSkillEarningEvent(
            IntermediateLearningAim intermediateLearningAim, ContractType contractType)
        {
            switch (contractType)
            {
                case ContractType.Act1:
                    return mapper.Map<Act1FunctionalSkillEarningsEvent>(intermediateLearningAim);
                case ContractType.Act2:
                    return mapper.Map<Act2FunctionalSkillEarningsEvent>(intermediateLearningAim);
            }

            return null;
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
    }
}