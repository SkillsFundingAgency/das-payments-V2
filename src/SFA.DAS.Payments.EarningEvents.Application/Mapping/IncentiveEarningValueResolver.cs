using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class IncentiveEarningValueResolver : IValueResolver<IntermediateLearningAim, ApprenticeshipContractTypeEarningsEvent, List<IncentiveEarning>>
    {

        // ReSharper disable once InconsistentNaming
        private static string IncentiveEarningTypeToFM36AttributeName(IncentiveEarningType incentiveEarningType)
        {
            switch (incentiveEarningType)
            {
                case IncentiveEarningType.First16To18EmployerIncentive:
                    return "PriceEpisodeFirstEmp1618Pay";
                case IncentiveEarningType.First16To18ProviderIncentive:
                    return "PriceEpisodeFirstProv1618Pay";
                case IncentiveEarningType.Second16To18EmployerIncentive:
                    return "PriceEpisodeSecondEmp1618Pay";
                case IncentiveEarningType.Second16To18ProviderIncentive:
                    return "PriceEpisodeSecondProv1618Pay";
                case IncentiveEarningType.OnProgramme16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftOnProgPayment";
                case IncentiveEarningType.Completion16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftCompletionPayment";
                case IncentiveEarningType.Balancing16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftBalancing";
                case IncentiveEarningType.FirstDisadvantagePayment:
                    return "PriceEpisodeFirstDisadvantagePayment";
                case IncentiveEarningType.SecondDisadvantagePayment:
                    return "PriceEpisodeSecondDisadvantagePayment";
                case IncentiveEarningType.LearningSupport:
                    return "PriceEpisodeLSFCash"; //TODO: Get definitive answer from Khush/David Young
                case IncentiveEarningType.CareLeaverApprenticePayment:
                    return "PriceEpisodeLearnerAdditionalPayment";

                default :
                    throw new InvalidOperationException($"Cannot get FM36 attribute name for unknown incentive earning type: {incentiveEarningType}");
            }
        }

        private IncentiveEarning CreateIncentiveEarning(List<PriceEpisode> priceEpisodes, IncentiveEarningType incentiveType)
        {
            var attributeName = IncentiveEarningTypeToFM36AttributeName(incentiveType);
            var allPeriods = priceEpisodes.Select(p => p.PriceEpisodePeriodisedValues.SingleOrDefault(v => v.AttributeName == attributeName))
                .Where(p => p != null)
                .ToArray();

            var periods = new EarningPeriod[12];

            for (byte i = 1; i <= 12; i++)
            {
                var periodValues = allPeriods.Select(p => p.GetPeriodValue(i)).ToArray();
                var periodValue = periodValues.SingleOrDefault(v => v.GetValueOrDefault(0) != 0).GetValueOrDefault(0);

                var priceEpisodeIdentifier = periodValue == 0 ? null : priceEpisodes[periodValues.IndexOf(periodValue)].PriceEpisodeIdentifier;
                var sfaContributionPercentage = priceEpisodes.CalculateSfaContributionPercentage(i, priceEpisodeIdentifier);

                periods[i - 1] = new EarningPeriod
                {
                    Period = i,
                    Amount = periodValue.AsRounded(),
                    PriceEpisodeIdentifier = priceEpisodeIdentifier,
                    SfaContributionPercentage = sfaContributionPercentage,
                };
            }
            var result = new IncentiveEarning
            {
                Type = incentiveType,
                Periods = new ReadOnlyCollection<EarningPeriod>(periods)
            };

            return result;
        }

        private static readonly IncentiveEarningType[] IncentiveTypes =
        {
            IncentiveEarningType.First16To18EmployerIncentive,
            IncentiveEarningType.First16To18ProviderIncentive,
            IncentiveEarningType.Second16To18EmployerIncentive,
            IncentiveEarningType.Second16To18ProviderIncentive,
            IncentiveEarningType.OnProgramme16To18FrameworkUplift,
            IncentiveEarningType.Completion16To18FrameworkUplift,
            IncentiveEarningType.Balancing16To18FrameworkUplift,
            IncentiveEarningType.FirstDisadvantagePayment,
            IncentiveEarningType.SecondDisadvantagePayment,
            IncentiveEarningType.LearningSupport,
            IncentiveEarningType.CareLeaverApprenticePayment,
        };

        public List<IncentiveEarning> Resolve(IntermediateLearningAim source, ApprenticeshipContractTypeEarningsEvent destination, List<IncentiveEarning> destMember, ResolutionContext context)
        {
            return IncentiveTypes
                .Select(type => CreateIncentiveEarning(source.PriceEpisodes, type))
                .Where(earning => earning.Periods.Any(x => x.Amount != 0))
                .ToList();
        }
    }
}