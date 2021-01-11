using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.Payments.Core;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class ApprenticeshipContractTypeEarningsEventBuilder : EarningEventBuilderBase,
        IApprenticeshipContractTypeEarningsEventBuilder
    {
        private readonly IApprenticeshipContractTypeEarningsEventFactory factory;
        private readonly IRedundancyEarningService redundancyEarningService;
        private readonly IMapper mapper;

        public ApprenticeshipContractTypeEarningsEventBuilder(IApprenticeshipContractTypeEarningsEventFactory factory,
            IRedundancyEarningService redundancyEarningService,
            IMapper mapper)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.redundancyEarningService = redundancyEarningService ?? throw new ArgumentNullException(nameof(redundancyEarningService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public List<ApprenticeshipContractTypeEarningsEvent> Build(ProcessLearnerCommand learnerSubmission)
        {
            var intermediateResults = InitialLearnerTransform(learnerSubmission, true);
            var results = new List<ApprenticeshipContractTypeEarningsEvent>();

            // Sort all p/e by start date
            var sortedPriceEpisodes = learnerSubmission.Learner.PriceEpisodes
                .OrderBy(x => x.PriceEpisodeValues.EpisodeStartDate)
                .ToList();
            
            foreach (var intermediateLearningAim in intermediateResults)
            {
                //var episodesByContractType = intermediateLearningAim.PriceEpisodes.GroupBy(x => new {x.PriceEpisodeValues.PriceEpisodeContractType, x.PriceEpisodeValues.PriceEpisodeRedStatusCode});
                var episodesByContractType = intermediateLearningAim.PriceEpisodes.GroupBy(x => x.PriceEpisodeValues.PriceEpisodeContractType);
                var redundancyDates = intermediateLearningAim.PriceEpisodes
                    .Where(pe => pe.PriceEpisodeValues.PriceEpisodeRedStatusCode == 1 && pe.PriceEpisodeValues.PriceEpisodeRedStartDate.HasValue)
                    .OrderBy(pe => pe.PriceEpisodeValues.PriceEpisodeRedStartDate)
                    .Select( pe =>new { pe.PriceEpisodeValues.PriceEpisodeRedStartDate, pe.PriceEpisodeIdentifier}).FirstOrDefault();
                //new variable that is list of redundancy periods


                foreach (var priceEpisodes in episodesByContractType)
                {
                    var learnerWithSortedPriceEpisodes =
                        intermediateLearningAim.CopyReplacingPriceEpisodes(priceEpisodes);

                     //var earningEvent = factory.Create(priceEpisodes.Key.PriceEpisodeContractType);
                     var earningEvent = factory.Create(priceEpisodes.Key);
                    if (!earningEvent.IsPayable) continue;

                    mapper.Map(learnerWithSortedPriceEpisodes, earningEvent);

                    if (redundancyDates != null && earningEvent.PriceEpisodes.Any(pe=>pe.Identifier == redundancyDates.PriceEpisodeIdentifier))
                    {
                        var redundancyPriceEpisode = learnerSubmission.Learner.PriceEpisodes.First(x => x.PriceEpisodeValues.PriceEpisodeRedStatusCode == 1 &&
                                                                                                        x.PriceEpisodeIdentifier == redundancyDates.PriceEpisodeIdentifier);
                        var index = sortedPriceEpisodes.IndexOf(redundancyPriceEpisode);
                        var nextStartDate = (DateTime?) null;
                        if (index < sortedPriceEpisodes.Count - 1) //todo fix scenario where multiple redundant price episodes causing the wrong one to be picked up here
                            nextStartDate = sortedPriceEpisodes[index + 1].PriceEpisodeValues.EpisodeStartDate;
                        results.AddRange(redundancyEarningService.SplitContractEarningByRedundancyDate(earningEvent, redundancyDates.PriceEpisodeRedStartDate.Value, nextStartDate));
                        //replace last 2 variables on line above with new list of redundancy periods and fix method to handle
                    }
                    else
                    {
                        results.Add(earningEvent);
                    }
                }
            }

            return results;
        }

        public List<byte> CalculateRedundancyPeriods(List<PriceEpisode> priceEpisodes)
        {
            if(priceEpisodes.All(x => x.PriceEpisodeValues.PriceEpisodeRedStatusCode != 1))
                return new List<byte>();

            var redundancyPeriods = new List<byte>();

            //order them
            var orderedPriceEpisodes = priceEpisodes.OrderBy(x => x.PriceEpisodeValues.EpisodeStartDate).ToList();
            var priceEpisodeCount = orderedPriceEpisodes.Count;

            for (var i = 0; i < priceEpisodeCount; i++)
            {
                var currentPriceEpisode = orderedPriceEpisodes[i];
                if (currentPriceEpisode.PriceEpisodeValues.PriceEpisodeRedStatusCode == 1)
                {
                    var startPeriod = currentPriceEpisode.PriceEpisodeValues.PriceEpisodeRedStartDate.Value.GetPeriodFromDate();
                    var nextPeriod = 13;
                    if (i + 1 < priceEpisodeCount)
                    {
                        nextPeriod = orderedPriceEpisodes[i + 1].PriceEpisodeValues.EpisodeStartDate.Value.GetPeriodFromDate();
                    }

                    for (byte j = startPeriod; j < nextPeriod; j++)
                    {
                        redundancyPeriods.Add(j);
                    }
                }
            }

            return redundancyPeriods;
        }
     }
}