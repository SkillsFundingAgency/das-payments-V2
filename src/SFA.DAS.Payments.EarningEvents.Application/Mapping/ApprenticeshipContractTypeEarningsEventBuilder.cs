using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

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
                        if (index < sortedPriceEpisodes.Count - 1)
                            nextStartDate = sortedPriceEpisodes[index + 1].PriceEpisodeValues.EpisodeStartDate;
                        results.AddRange(redundancyEarningService.SplitContractEarningByRedundancyDate(earningEvent, redundancyDates.PriceEpisodeRedStartDate.Value, nextStartDate));
                    }
                    else
                    {
                        results.Add(earningEvent);
                    }
                }
            }

            return results;
        }
     }
}