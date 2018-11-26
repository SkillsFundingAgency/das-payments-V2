using System;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Newtonsoft.Json;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class IncentiveEarningValueResolver : IValueResolver<ProcessLearnerCommand, ApprenticeshipContractTypeEarningsEvent, ReadOnlyCollection<IncentiveEarning>>
    {

        // ReSharper disable once InconsistentNaming
        private static string IncentiveEarningTypeToFM36AttributeName(IncentiveType incentiveEarningType)
        {
            switch (incentiveEarningType)
            {
                case IncentiveType.First16To18EmployerIncentive:
                    return "PriceEpisodeFirstEmp1618Pay";
                case IncentiveType.First16To18ProviderIncentive:
                    return "PriceEpisodeFirstProv1618Pay";
                case IncentiveType.Second16To18EmployerIncentive:
                    return "PriceEpisodeSecondEmp1618Pay";
                case IncentiveType.Second16To18ProviderIncentive:
                    return "PriceEpisodeSecondProv1618Pay";
                case IncentiveType.OnProgramme16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftOnProgPayment";
                case IncentiveType.Completion16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftCompletionPayment";
                case IncentiveType.Balancing16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftBalancing";
                case IncentiveType.FirstDisadvantagePayment:
                    return "PriceEpisodeFirstDisadvantagePayment";
                case IncentiveType.SecondDisadvantagePayment:
                    return "PriceEpisodeSecondDisadvantagePayment";
                case IncentiveType.LearningSupport:
                    return "PriceEpisodeLSFCash"; //TODO: Get definitive answer from Khush/David Young

                default :
                    throw new InvalidOperationException($"Cannot get FM36 attribute name for unknown incentive earning type: {incentiveEarningType}");
            }
        }

        private IncentiveEarning CreateIncentiveEarning(FM36Learner source, IncentiveType incentiveType)
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
                    .SelectMany(priceEpisodeValues => priceEpisodeValues.periodisedValues.CreateEarningPeriods(priceEpisodeValues.priceEpisode.PriceEpisodeIdentifier))
                    .ToList()
                    .AsReadOnly()
            };

            return result;
        }

        private static readonly IncentiveType[] IncentiveTypes =
        {
            IncentiveType.First16To18EmployerIncentive,
            IncentiveType.First16To18EmployerIncentive,
            IncentiveType.First16To18ProviderIncentive,
            IncentiveType.Second16To18EmployerIncentive,
            IncentiveType.Second16To18ProviderIncentive,
            IncentiveType.OnProgramme16To18FrameworkUplift,
            IncentiveType.Completion16To18FrameworkUplift,
            IncentiveType.Balancing16To18FrameworkUplift,
            IncentiveType.FirstDisadvantagePayment,
            IncentiveType.FirstDisadvantagePayment,
            IncentiveType.SecondDisadvantagePayment,
            IncentiveType.LearningSupport
        };

        public ReadOnlyCollection<IncentiveEarning> Resolve(ProcessLearnerCommand source, ApprenticeshipContractTypeEarningsEvent destination, ReadOnlyCollection<IncentiveEarning> destMember, ResolutionContext context)
        {

            return IncentiveTypes
                .Select(type => CreateIncentiveEarning(source.Learner, type))
                .Where(earning => earning.Periods.Any())
                .ToList()
                .AsReadOnly();
        }
    }
}