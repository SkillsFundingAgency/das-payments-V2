using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Entities;

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
                var functionalSkillEarning = mapper.Map<FunctionalSkillEarningsEvent>(learnerWithSortedPriceEpisodes);
                if (learnerSubmission.Learner.PriceEpisodes.First().PriceEpisodeValues.PriceEpisodeContractType.Equals("Levy Contract", StringComparison.OrdinalIgnoreCase))
                {
                    functionalSkillEarning.ContractType = ContractType.Act1;
                }
                else
                {
                    functionalSkillEarning.ContractType = ContractType.Act2;
                }
                results.Add(functionalSkillEarning);
            }

            return results;
        }
    }
}