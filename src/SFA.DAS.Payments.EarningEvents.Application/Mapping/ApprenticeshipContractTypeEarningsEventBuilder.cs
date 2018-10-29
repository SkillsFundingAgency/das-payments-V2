using System;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class ApprenticeshipContractTypeEarningsEventBuilder : IApprenticeshipContractTypeEarningsEventBuilder
    {
        private readonly IApprenticeshipContractTypeEarningsEventFactory factory;
        private readonly IMapper mapper;

        public ApprenticeshipContractTypeEarningsEventBuilder(IApprenticeshipContractTypeEarningsEventFactory factory, IMapper mapper)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ApprenticeshipContractTypeEarningsEvent Build(ProcessLearnerCommand learnerSubmission)
        {
            var contractType = learnerSubmission.Learner.PriceEpisodes.GetLatestPriceEpisode()
                ?.PriceEpisodeValues.PriceEpisodeContractType ?? throw new InvalidOperationException($"Failed to find the contract type for ilr learner: {learnerSubmission.Learner.LearnRefNumber}");

            var earningEvent = factory.Create(contractType);
             mapper.Map(learnerSubmission,earningEvent);
            return earningEvent;
        }
    }
}