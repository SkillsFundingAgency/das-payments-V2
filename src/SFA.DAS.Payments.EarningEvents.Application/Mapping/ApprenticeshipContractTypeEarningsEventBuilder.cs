using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class ApprenticeshipContractTypeEarningsEventBuilder : EarningEventBuilderBase, IApprenticeshipContractTypeEarningsEventBuilder
    {
        private readonly IApprenticeshipContractTypeEarningsEventFactory factory;
        private readonly IMapper mapper;

        public ApprenticeshipContractTypeEarningsEventBuilder(IApprenticeshipContractTypeEarningsEventFactory factory, IMapper mapper)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public List<ApprenticeshipContractTypeEarningsEvent> Build(ProcessLearnerCommand learnerSubmission)
        {
            var intermediateResults = InitialLearnerTransform(learnerSubmission, true);
            var results = new List<ApprenticeshipContractTypeEarningsEvent>();

            foreach (var intermediateLearningAim in intermediateResults)
            {
                var episodesByContractType = intermediateLearningAim.PriceEpisodes.GroupBy(x => x.PriceEpisodeValues.PriceEpisodeContractType);

                foreach (var priceEpisodes in episodesByContractType)
                {
                    var learnerWithSortedPriceEpisodes = intermediateLearningAim.CopyReplacingPriceEpisodes(priceEpisodes);

                    var earningEvent = factory.Create(priceEpisodes.Key);
                    if (!earningEvent.IsApplicableContractType) continue;

                    mapper.Map(learnerWithSortedPriceEpisodes, earningEvent);

                    var priceEpisodeToUse = priceEpisodes.FirstOrDefault() ??
                                            throw new NullReferenceException("Price Episode Cannot be null");

                    earningEvent.LearningAim.FundingLineType = priceEpisodeToUse.PriceEpisodeValues.PriceEpisodeFundLineType;

                    results.Add(earningEvent);
                }
            }

            return results;
        }
    }
}