using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class ApprenticeshipContractTypeEarningsEventBuilder : EarningEventBuilderBase, IApprenticeshipContractTypeEarningsEventBuilder
    {
        private readonly IApprenticeshipContractTypeEarningsEventFactory factory;
        private readonly IMapper mapper;
       
        public ApprenticeshipContractTypeEarningsEventBuilder(IApprenticeshipContractTypeEarningsEventFactory factory, IMapper mapper, IConfigurationHelper configurationHelper)
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
                    mapper.Map(learnerWithSortedPriceEpisodes, earningEvent);
                    
                    results.Add(earningEvent);
                }
            }
            
            return results;
        }
    }
}