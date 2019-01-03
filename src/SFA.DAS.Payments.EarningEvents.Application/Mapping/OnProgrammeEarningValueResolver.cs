using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FastMember;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public static class SfaContributionPercentageExtensions
    {
        private static readonly TypeAccessor PeriodAccessor = TypeAccessor.Create(typeof(PriceEpisodePeriodisedValues));

        public static decimal? CalculateSfaContributionPercentage(this List<PriceEpisode> source, int period,
            string priceEpisodeIdentifier)
        {
            if (priceEpisodeIdentifier == null)
            {
                return null;
            }
            var priceEpisode = source.Single(x => x.PriceEpisodeIdentifier == priceEpisodeIdentifier);
            var value = priceEpisode.PriceEpisodePeriodisedValues
                .Where(x => x.AttributeName == "PriceEpisodeSFAContribPct")
                .Select(p => (decimal?)PeriodAccessor[p, "Period" + period])
                .SingleOrDefault();

            if (value == null)
            {
                value = priceEpisode.PriceEpisodeValues.PriceEpisodeSFAContribPct;
            }

            return value;
        }
    }

    public class OnProgrammeEarningValueResolver : IValueResolver<IntermediateLearningAim, ApprenticeshipContractTypeEarningsEvent, ReadOnlyCollection<OnProgrammeEarning>>
    {
        // ReSharper disable once InconsistentNaming
        private static string OnProgrammeEarningTypeToFM36AttributeName(OnProgrammeEarningType onProgrammeEarningType)
        {
            switch (onProgrammeEarningType)
            {
                case OnProgrammeEarningType.Balancing:
                    return "PriceEpisodeBalancePayment";
                case OnProgrammeEarningType.Completion:
                    return "PriceEpisodeCompletionPayment";
                case OnProgrammeEarningType.Learning:
                    return "PriceEpisodeOnProgPayment";
                default:
                    throw new InvalidOperationException($"Cannot get FM36 attribute name for unknown on-programme earning type: {onProgrammeEarningType}");
            }
        }

        private OnProgrammeEarning CreateOnProgrammeEarning(IntermediateLearningAim source, OnProgrammeEarningType onProgrammeEarningType)
        {
            var attributeName = OnProgrammeEarningTypeToFM36AttributeName(onProgrammeEarningType);
            var allPeriods = source.PriceEpisodes.Select(p => p.PriceEpisodePeriodisedValues.SingleOrDefault(v => v.AttributeName == attributeName))
                .Where(p => p != null)
                .ToArray();
            
            var periods = new EarningPeriod[12];

            for (byte i = 1; i <= 12; i++)
            {
                var periodValues = allPeriods.Select(p => p.GetPeriodValue(i)).ToArray();
                var periodValue = periodValues.SingleOrDefault(v => v.GetValueOrDefault(0) != 0).GetValueOrDefault(0);
                var priceEpisodeIdentifier = periodValue == 0 ? null : source.PriceEpisodes[periodValues.IndexOf(periodValue)].PriceEpisodeIdentifier;
                var sfaContributionPercentage = source.PriceEpisodes.CalculateSfaContributionPercentage(i, priceEpisodeIdentifier);

                periods[i - 1] = new EarningPeriod
                {
                    Period = i,
                    Amount = periodValue,
                    PriceEpisodeIdentifier = priceEpisodeIdentifier,
                    SfaContributionPercentage = sfaContributionPercentage,
                };
            }

            return new OnProgrammeEarning
            {
                Type = onProgrammeEarningType,
                Periods = new ReadOnlyCollection<EarningPeriod>(periods)
            };
        }

        private static readonly OnProgrammeEarningType[] OnProgrammeEarningTypes = { OnProgrammeEarningType.Balancing, OnProgrammeEarningType.Completion, OnProgrammeEarningType.Learning };

        public ReadOnlyCollection<OnProgrammeEarning> Resolve(IntermediateLearningAim source, ApprenticeshipContractTypeEarningsEvent destination, ReadOnlyCollection<OnProgrammeEarning> destMember, ResolutionContext context)
        {
            return OnProgrammeEarningTypes
                .Select(type => CreateOnProgrammeEarning(source, type))
                .ToList()
                .AsReadOnly();
        }
    }
}