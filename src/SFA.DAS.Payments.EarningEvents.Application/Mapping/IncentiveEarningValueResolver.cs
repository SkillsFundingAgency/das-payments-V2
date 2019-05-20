using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Incentives;

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

        private IncentiveEarning CreateIncentiveEarning(FM36Learner source, IncentiveEarningType incentiveType)
        {
            var result = new IncentiveEarning
            {
                Type = incentiveType,
                Periods = source.PriceEpisodes
                    .Where(priceEpisode => priceEpisode.PriceEpisodePeriodisedValues != null)
                    .SelectMany(priceEpisode => priceEpisode.PriceEpisodePeriodisedValues,
                        (priceEpisode, periodisedValues) => new { priceEpisode, periodisedValues })
                    .Where(priceEpisodeValues =>
                        priceEpisodeValues.periodisedValues.AttributeName == IncentiveEarningTypeToFM36AttributeName(incentiveType))
                    .SelectMany(priceEpisodeValues => priceEpisodeValues.periodisedValues.CreateIncentiveEarningPeriods(priceEpisodeValues.priceEpisode.PriceEpisodeIdentifier))
                    .ToList()
                    .AsReadOnly()
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
                .Select(type => CreateIncentiveEarning(source.Learner, type))
                .Where(earning => earning.Periods.Any())
                .ToList();
        }
    }
}