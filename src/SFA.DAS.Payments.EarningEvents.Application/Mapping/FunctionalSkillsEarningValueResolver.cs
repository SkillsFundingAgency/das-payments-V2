using System;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class FunctionalSkillsEarningValueResolver : IValueResolver<IntermediateLearningAim, FunctionalSkillEarningsEvent, ReadOnlyCollection<FunctionalSkillEarning>>
    {
        private static readonly FunctionalSkillType[] FunctionalSkillEarningTypes = { FunctionalSkillType.BalancingMathsAndEnglish, FunctionalSkillType.OnProgrammeMathsAndEnglish, FunctionalSkillType.LearningSupport };

        private static string FunctionalSkillEarningTypeToFM36AttributeName(FunctionalSkillType functionalSkillEarningType)
        {
            switch (functionalSkillEarningType)
            {
                case FunctionalSkillType.BalancingMathsAndEnglish:
                    return "MathEngBalPayment";
                case FunctionalSkillType.OnProgrammeMathsAndEnglish:
                    return "MathEngOnProgPayment";
                case FunctionalSkillType.LearningSupport:
                    return "LearnSuppFundCash";
                default:
                    throw new InvalidOperationException($"Cannot get FM36 attribute name for unknown functional skill earning type: {functionalSkillEarningType}");
            }
        }
        public ReadOnlyCollection<FunctionalSkillEarning> Resolve(IntermediateLearningAim source, FunctionalSkillEarningsEvent destination, ReadOnlyCollection<FunctionalSkillEarning> destMember, ResolutionContext context)
        {
            return FunctionalSkillEarningTypes
                .Select(type => CreateEarning(source, type))
                .ToList()
                .AsReadOnly();
        }

        private FunctionalSkillEarning CreateEarning(IntermediateLearningAim source, FunctionalSkillType functionalSkillEarningType)
        {
            var attributeName = FunctionalSkillEarningTypeToFM36AttributeName(functionalSkillEarningType);
            var allPeriods = source.Aims.Select(p => p.LearningDeliveryPeriodisedValues.SingleOrDefault(v => v.AttributeName == attributeName))
                .Where(p => p != null)
                .ToArray();

            var periods = new EarningPeriod[12];

            for (byte i = 1; i <= 12; i++)
            {
                var periodValues = allPeriods.Select(p => p.GetPeriodValue(i)).ToArray();

                var periodValue = 0m;
                try
                {
                    periodValue = periodValues.SingleOrDefault(v => v.GetValueOrDefault(0) != 0).GetValueOrDefault(0);
                }
                catch
                {
                    // ignored
                }

                periods[i - 1] = new EarningPeriod
                {
                    Period = i,
                    Amount = periodValue.AsRounded(),
                    SfaContributionPercentage = 1
                };
            }

            return new FunctionalSkillEarning
            {
                Type = functionalSkillEarningType,
                Periods = new ReadOnlyCollection<EarningPeriod>(periods)
            };
        }
    }
}