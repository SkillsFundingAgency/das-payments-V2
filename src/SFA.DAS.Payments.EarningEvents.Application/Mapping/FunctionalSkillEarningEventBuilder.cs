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
                if (intermediateLearningAim.Aim.IsMainAim())
                {
                    continue;
                }

                var contractTypes =   intermediateLearningAim.Aim.GetContractTypesForLearningDeliveries();

                var distinctContractTypes = contractTypes.Where(x=> x != ContractType.None).Distinct().ToList();

                foreach (var contractType in distinctContractTypes)
                {
                    var functionalSkillEarning = GetContractTypeFunctionalSkillEarningEvent(intermediateLearningAim, contractType);

                    foreach (var earning in functionalSkillEarning.Earnings)
                    {
                        earning.Periods = GetEarningPeriodsMatchingContractType(contractTypes, contractType, earning.Periods.ToList());
                    }

                    if(!functionalSkillEarning.Earnings.SelectMany(x => x.Periods).Any()) continue;

                    foreach (var earning in functionalSkillEarning.Earnings)
                    {
                        if (!earning.Periods.Any() || earning.Periods.Count == 12) continue;

                        var earningPeriods = new List<EarningPeriod>(earning.Periods);

                        var sfaContribution = earning.Periods.First().SfaContributionPercentage;

                        for (byte i = 1; i < 13; i++)
                        {
                            if (earning.Periods.All(x => x.Period != i))
                            {
                                earningPeriods.Add(new EarningPeriod
                                {
                                    Amount = 0.0m,
                                    SfaContributionPercentage = sfaContribution,
                                    Period =  i,
                                });
                            }
                        }

                        earning.Periods = earningPeriods.AsReadOnly();
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