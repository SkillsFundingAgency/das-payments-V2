using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Application.Messages;
using SFA.DAS.Payments.EarningEvents.Domain;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class OnProgrammeEarningValueResolver : IValueResolver<IIlrLearnerSubmission, ApprenticeshipContractTypeEarningsEvent, List<OnProgrammeEarning>>
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

        private OnProgrammeEarning CreateOnProgrammeEarning(FM36Learner source, OnProgrammeEarningType onProgrammeEarningType)
        {
            return new OnProgrammeEarning
            {
                Type = onProgrammeEarningType,
                Periods = source.PriceEpisodes
                    .Where(priceEpisode => priceEpisode.PriceEpisodePeriodisedValues != null)
                    .SelectMany(priceEpisode => priceEpisode.PriceEpisodePeriodisedValues,
                        (priceEpisode, periodisedValues) => new { priceEpisode, periodisedValues })
                    .Where(priceEpisodeValues =>
                        priceEpisodeValues.periodisedValues.AttributeName == OnProgrammeEarningTypeToFM36AttributeName(onProgrammeEarningType))
                    .SelectMany(priceEpisodeValues => priceEpisodeValues.periodisedValues.CreateEarningPeriods(priceEpisodeValues.priceEpisode.PriceEpisodeIdentifier) )
                    .ToList()
                    .AsReadOnly()
            };
        }

        private static readonly OnProgrammeEarningType[] OnProgrammeEarningTypes = new OnProgrammeEarningType[] { OnProgrammeEarningType.Balancing, OnProgrammeEarningType.Completion, OnProgrammeEarningType.Learning };
        public List<OnProgrammeEarning> Resolve(IIlrLearnerSubmission source, ApprenticeshipContractTypeEarningsEvent destination, List<OnProgrammeEarning> destMember, ResolutionContext context)
        {
            return OnProgrammeEarningTypes
                .Select(type => CreateOnProgrammeEarning(source.Learner, type))
                .Where(earning => earning.Periods.Any())
                .ToList();
        }
    }
}