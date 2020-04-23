using System;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class RedundancyEarningEventFactory : IRedundancyEarningEventFactory
    {
        private readonly IMapper mapper;

        public RedundancyEarningEventFactory(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ApprenticeshipContractTypeEarningsEvent CreateRedundancyContractTypeEarningsEvent(
            ApprenticeshipContractTypeEarningsEvent earningEvent)
        {
            if (earningEvent is ApprenticeshipContractType1EarningEvent act1)
                return mapper.Map<ApprenticeshipContractType1RedundancyEarningEvent>(act1);

            if (earningEvent is ApprenticeshipContractType2EarningEvent act2)
                return mapper.Map<ApprenticeshipContractType2RedundancyEarningEvent>(act2);

            return null;
        }

        public FunctionalSkillEarningsEvent CreateRedundancyFunctionalSkillTypeEarningsEvent(
            FunctionalSkillEarningsEvent functionalSkillEarning)
        {
            if (functionalSkillEarning is Act1FunctionalSkillEarningsEvent act1)
                return mapper.Map<Act1RedundancyFunctionalSkillEarningsEvent>(act1);

            if (functionalSkillEarning is Act2FunctionalSkillEarningsEvent act2)
                return mapper.Map<Act2RedundancyFunctionalSkillEarningsEvent>(act2);

            return null;
        }
    }
}