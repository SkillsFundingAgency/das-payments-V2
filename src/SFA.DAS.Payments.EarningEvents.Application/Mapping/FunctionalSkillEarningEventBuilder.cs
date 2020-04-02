using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
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
        private readonly IRedundancyEarningSplitter redundancyEarningSplitter;

        public FunctionalSkillEarningEventBuilder(IMapper mapper, IRedundancyEarningSplitter redundancyEarningSplitter)
        {
            this.mapper = mapper;
            this.redundancyEarningSplitter = redundancyEarningSplitter ?? throw new ArgumentNullException(nameof(redundancyEarningSplitter));
        }

        public List<FunctionalSkillEarningsEvent> Build(ProcessLearnerCommand learnerSubmission)
        {
            var intermediateResults = InitialLearnerTransform(learnerSubmission, false);
            var results = new List<FunctionalSkillEarningsEvent>();

            foreach (var intermediateLearningAim in intermediateResults)
            {
                var redundancyDates = intermediateLearningAim.PriceEpisodes
                    .Where(pe => pe.PriceEpisodeValues.PriceEpisodeRedStatusCode == 1)
                    .Select( pe =>new { pe.PriceEpisodeValues.PriceEpisodeRedStartDate, pe.PriceEpisodeIdentifier}).FirstOrDefault();



                if (intermediateLearningAim.Aims.All(x => x.IsMainAim()))
                {
                    continue;
                }

                var contractTypes = intermediateLearningAim.Aims.GetContractTypesForLearningDeliveries();

                var distinctContractTypes = contractTypes.Where(x => x != ContractType.None).Distinct().ToList();

                foreach (var contractType in distinctContractTypes)
                {
                    var functionalSkillEarning = GetContractTypeFunctionalSkillEarningEvent(intermediateLearningAim, contractType);

                    foreach (var earning in functionalSkillEarning.Earnings)
                    {
                        earning.Periods = GetEarningPeriodsMatchingContractType(contractTypes, contractType, earning.Periods.ToList());
                    }

                    if (!functionalSkillEarning.Earnings.SelectMany(x => x.Periods).Any()) continue;

                    functionalSkillEarning.LearningAim.FundingLineType = GetFirstMatchingFundingLineTypeForContractType(intermediateLearningAim, functionalSkillEarning);

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
                                    Period = i,
                                });
                            }
                        }

                        earning.Periods = earningPeriods.AsReadOnly();
                    }
                    if (redundancyDates != null && functionalSkillEarning.PriceEpisodes.Any(pe=>pe.Identifier == redundancyDates.PriceEpisodeIdentifier))
                    {
                        results.AddRange(redundancyEarningSplitter.SplitFunctionSkillEarningByRedundancyDate(functionalSkillEarning, redundancyDates.PriceEpisodeRedStartDate.Value));
                    }
                    else
                    {
                        results.Add(functionalSkillEarning);
                    }
                 
                }
            }

            return results.Distinct().ToList();
        }

        private static string GetFirstMatchingFundingLineTypeForContractType(IntermediateLearningAim intermediateLearningAim, FunctionalSkillEarningsEvent functionalSkillEarning)
        {
            var periodisedFundingLineTypeValues = intermediateLearningAim
                .Aims
                .SelectMany(x => x.LearningDeliveryPeriodisedTextValues)
                .Where(x => x.AttributeName.Equals("FundLineType"))
                .Select(x => x);

            if (periodisedFundingLineTypeValues != null)
            {
                var periodWithActiveEarning = functionalSkillEarning
                    .Earnings
                    .SelectMany(x => x.Periods)
                    .Select(x => x.Period)
                    .First();

                var fundingLineType = periodisedFundingLineTypeValues
                .Select(x => x.GetPeriodTextValue(periodWithActiveEarning))
                .FirstOrDefault(f => !f.Equals(GlobalConstants.Fm36NoneText, StringComparison.OrdinalIgnoreCase));
               
                return fundingLineType;
            }

            throw new InvalidOperationException($"Can't find a valid FundingLineType for aim {intermediateLearningAim.Aims.First().LearningDeliveryValues.LearnAimRef}");
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