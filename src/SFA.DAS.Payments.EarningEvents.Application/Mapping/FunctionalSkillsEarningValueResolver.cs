using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class FunctionalSkillsEarningValueResolver : IValueResolver<IntermediateLearningAim, FunctionalSkillEarningsEvent, ReadOnlyCollection<FunctionalSkillEarning>>
    {
        private static readonly Dictionary<string, FunctionalSkillType> TypeMap = new Dictionary<string, FunctionalSkillType>
        {
            {"MathEngBalPayment", FunctionalSkillType.BalancingMathsAndEnglish},
            {"MathEngOnProgPayment", FunctionalSkillType.OnProgrammeMathsAndEnglish}
        };

        public ReadOnlyCollection<FunctionalSkillEarning> Resolve(IntermediateLearningAim source, FunctionalSkillEarningsEvent destination, ReadOnlyCollection<FunctionalSkillEarning> destMember, ResolutionContext context)
        {
            return source.PriceEpisodes.SelectMany(priceEpisode => priceEpisode.PriceEpisodePeriodisedValues)
                .Where(periodisedValues => TypeMap.ContainsKey(periodisedValues.AttributeName))
                .GroupBy(v => v.AttributeName)
                .Select(values => CreateEarning(source, values))
                .ToList()
                .AsReadOnly();
        }

        private FunctionalSkillEarning CreateEarning(IntermediateLearningAim source, IGrouping<string, PriceEpisodePeriodisedValues> grouping)
        {
            if (grouping.Count() > 1)
                throw new ArgumentException("More than one functional skill earning of type " + grouping.Key);

            var allPeriods = source.PriceEpisodes.Select(p => p.PriceEpisodePeriodisedValues.SingleOrDefault(v => v.AttributeName == grouping.Key))
                .Where(p => p != null)
                .ToArray();

            var periods = new EarningPeriod[12];

            for (byte i = 1; i <= 12; i++)
            {
                var periodValues = allPeriods.Select(p => p.GetPeriodValue(i)).ToArray();
                var periodValue = periodValues.SingleOrDefault(v => v.GetValueOrDefault(0) != 0).GetValueOrDefault(0);
                var priceEpisodeIdentifier = periodValue == 0 ? null : source.PriceEpisodes[periodValues.IndexOf(periodValue)].PriceEpisodeIdentifier;

                periods[i - 1] = new EarningPeriod
                {
                    Period = i,
                    Amount = periodValue,
                    PriceEpisodeIdentifier = priceEpisodeIdentifier
                };
            }

            return new FunctionalSkillEarning
            {
                Type = TypeMap[grouping.Key],
                Periods = new ReadOnlyCollection<EarningPeriod>(periods)
            };
        }
    }
}