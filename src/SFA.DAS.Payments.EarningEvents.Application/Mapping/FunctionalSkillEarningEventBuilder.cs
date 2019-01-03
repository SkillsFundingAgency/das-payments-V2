using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class FunctionalSkillEarningEventBuilder : EarningEventBuilderBase, IFunctionalSkillEarningsEventBuilder
    {
        private readonly IMapper mapper;

        public FunctionalSkillEarningEventBuilder(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public List<FunctionalSkillEarningsEvent> Build(ProcessLearnerCommand learnerSubmission)
        {
            var intermediateResults = InitialLearnerTransform(learnerSubmission, false);
            var results = new List<FunctionalSkillEarningsEvent>();

            foreach (var intermediateLearningAim in intermediateResults)
            {
                var learnerWithSortedPriceEpisodes = intermediateLearningAim.CopyReplacingPriceEpisodes(intermediateLearningAim.PriceEpisodes);
                results.Add(mapper.Map<FunctionalSkillEarningsEvent>(learnerWithSortedPriceEpisodes));
            }

            return results;
        }

    }
}