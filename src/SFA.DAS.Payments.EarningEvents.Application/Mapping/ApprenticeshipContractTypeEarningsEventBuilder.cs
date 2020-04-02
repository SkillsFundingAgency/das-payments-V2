using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class ApprenticeshipContractTypeEarningsEventBuilder : EarningEventBuilderBase,
        IApprenticeshipContractTypeEarningsEventBuilder
    {
        private readonly IApprenticeshipContractTypeEarningsEventFactory factory;
        private readonly IRedundancyEarningSplitter redundancyEarningSplitter;
        private readonly IMapper mapper;

        public ApprenticeshipContractTypeEarningsEventBuilder(IApprenticeshipContractTypeEarningsEventFactory factory,
            IRedundancyEarningSplitter redundancyEarningSplitter,
            IMapper mapper)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.redundancyEarningSplitter = redundancyEarningSplitter ?? throw new ArgumentNullException(nameof(redundancyEarningSplitter));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public List<ApprenticeshipContractTypeEarningsEvent> Build(ProcessLearnerCommand learnerSubmission)
        {
            var intermediateResults = InitialLearnerTransform(learnerSubmission, true);
            var results = new List<ApprenticeshipContractTypeEarningsEvent>();

            foreach (var intermediateLearningAim in intermediateResults)
            {
                var episodesByContractType = intermediateLearningAim.PriceEpisodes.GroupBy(x =>x.PriceEpisodeValues.PriceEpisodeContractType );
                var redundancyDates = intermediateLearningAim.PriceEpisodes
                    .Where(pe => pe.PriceEpisodeValues.PriceEpisodeRedStatusCode == 1)
                    .Select( pe =>new { pe.PriceEpisodeValues.PriceEpisodeRedStartDate, pe.PriceEpisodeIdentifier}).FirstOrDefault();


                foreach (var priceEpisodes in episodesByContractType)
                {
                    var learnerWithSortedPriceEpisodes =
                        intermediateLearningAim.CopyReplacingPriceEpisodes(priceEpisodes);

                    var earningEvent = factory.Create(priceEpisodes.Key);
                    if (!earningEvent.IsPayable) continue;

                    mapper.Map(learnerWithSortedPriceEpisodes, earningEvent);

                    if (redundancyDates != null && earningEvent.PriceEpisodes.Any(pe=>pe.Identifier == redundancyDates.PriceEpisodeIdentifier))
                    {
                        results.AddRange(redundancyEarningSplitter.SplitContractEarningByRedundancyDate(earningEvent, redundancyDates.PriceEpisodeRedStartDate.Value));
                    }
                    else
                    {
                        results.Add(earningEvent);
                    }
                }
            }

            return results;
        }

        private void AdjustRedundancyPayments(DateTime? priceEpisodeRedStartDate, ApprenticeshipContractTypeEarningsEvent earningEvent)
        {
            if (!priceEpisodeRedStartDate.HasValue)
            {
                throw new ArgumentOutOfRangeException(nameof(priceEpisodeRedStartDate));
            }

            var relevantPriceEpisode = earningEvent.PriceEpisodes.SingleOrDefault(pe =>
                pe.StartDate <= priceEpisodeRedStartDate.Value
                &&
                pe.PlannedEndDate >= priceEpisodeRedStartDate.Value);


            if (relevantPriceEpisode == null)
            {
                return;
            }

            var remainingTime = relevantPriceEpisode.PlannedEndDate -
                                priceEpisodeRedStartDate.Value;


            var onProgrammeEarning =
                earningEvent.OnProgrammeEarnings.FirstOrDefault(ope => ope.Type == OnProgrammeEarningType.Learning);

            if (onProgrammeEarning != null)
            {
                //adjust earnings of current period to pay full amount 
               var currentPeriods =   onProgrammeEarning.Periods;

               var collectionPeriod = earningEvent.CollectionPeriod;
               var currentEarningPeriod = currentPeriods.FirstOrDefault(cp => cp.Period == collectionPeriod.Period);
               if (currentEarningPeriod != null)
               {
                   //pay installment amount at full value
                       currentEarningPeriod.Amount = relevantPriceEpisode.InstalmentAmount;
                       currentEarningPeriod.SfaContributionPercentage = 1m;
               }
            }
        }
    }
}