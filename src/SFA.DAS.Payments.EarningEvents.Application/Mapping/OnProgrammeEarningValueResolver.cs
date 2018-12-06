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

    public class OnProgrammeEarningValueResolver : IValueResolver<IntermediateLearningAim, ApprenticeshipContractTypeEarningsEvent, ReadOnlyCollection<OnProgrammeEarning>>
    {
        private static readonly TypeAccessor periodAccessor = TypeAccessor.Create(typeof(PriceEpisodePeriodisedValues));

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

        private OnProgrammeEarning CreateOnProgrammeEarning(FM36Learner source, OnProgrammeEarningType onProgrammeEarningType)
        {
            var attributeName = OnProgrammeEarningTypeToFM36AttributeName(onProgrammeEarningType);
            var allPeriods = source.PriceEpisodes.Select(p => p.PriceEpisodePeriodisedValues.SingleOrDefault(v => v.AttributeName == attributeName))
                .Where(p => p != null)
                .ToArray();

            var periods = new EarningPeriod[12];

            for (byte i = 1; i <= 12; i++)
            {
                var periodValues = allPeriods.Select(p => (decimal?) periodAccessor[p, "Period" + i]).ToArray();
                var periodValue = periodValues.SingleOrDefault(v => v.GetValueOrDefault(0) != 0).GetValueOrDefault(0);
                var priceEpisodeIdentifier = periodValue == 0 ? null : source.PriceEpisodes[periodValues.IndexOf(periodValue)].PriceEpisodeIdentifier;

                periods[i - 1] = new EarningPeriod
                {
                    Period = i,
                    Amount = periodValue,
                    PriceEpisodeIdentifier = priceEpisodeIdentifier
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
                .Select(type => CreateOnProgrammeEarning(source.Learner, type))
                .ToList()
                .AsReadOnly();
        }
    }
}