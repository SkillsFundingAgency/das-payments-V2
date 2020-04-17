using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.OnProgramme;

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
                        results.AddRange(redundancyEarningService.SplitContractEarningByRedundancyDate(earningEvent, redundancyDates.PriceEpisodeRedStartDate.Value));
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