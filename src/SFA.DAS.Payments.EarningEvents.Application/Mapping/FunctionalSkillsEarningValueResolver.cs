using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class FunctionalSkillsEarningValueResolver : IValueResolver<IntermediateLearningAim, FunctionalSkillEarningsEvent, ReadOnlyCollection<FunctionalSkillEarning>>
    {
        private static readonly Dictionary<string, FunctionalSkillType> TypeMap =
            new Dictionary<string, FunctionalSkillType>
            {
                {"MathEngBalPayment", FunctionalSkillType.BalancingMathsAndEnglish},
                {"MathEngOnProgPayment", FunctionalSkillType.OnProgrammeMathsAndEnglish},
                {"PriceEpisodeLSFCash", FunctionalSkillType.LearningSupport},
            };

        public ReadOnlyCollection<FunctionalSkillEarning> Resolve(IntermediateLearningAim source, FunctionalSkillEarningsEvent destination, ReadOnlyCollection<FunctionalSkillEarning> destMember, ResolutionContext context)
        {
            return source.Aim.LearningDeliveryPeriodisedValues
                .Where(periodisedValues => TypeMap.ContainsKey(periodisedValues.AttributeName))
                .GroupBy(v => v.AttributeName)
                .Select(CreateEarning)
                .ToList()
                .AsReadOnly();
        }

        private FunctionalSkillEarning CreateEarning(IGrouping<string, LearningDeliveryPeriodisedValues> groupItem)
        {
            if (groupItem.Count() > 1)
                throw new ArgumentException($"More than one functional skill earning of type {groupItem.Key}");

            const byte earningPeriods = 12;

            var periods = new EarningPeriod[earningPeriods];

            for (byte i = 1; i <= earningPeriods; i++)
            {
                var periodValues = groupItem.Select(p => p.GetPeriodValue(i)).ToArray();
                var periodValue = periodValues.SingleOrDefault(v => v.GetValueOrDefault(0) != 0).GetValueOrDefault(0);
                
                periods[i - 1] = new EarningPeriod
                {
                    Period = i,
                    Amount = periodValue,
                    SfaContributionPercentage = 1,
                };
            }

            return new FunctionalSkillEarning
            {
                Type = TypeMap[groupItem.Key],
                Periods = new ReadOnlyCollection<EarningPeriod>(periods),
            };
        }
    }
}