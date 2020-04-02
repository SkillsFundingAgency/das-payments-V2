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

        public ApprenticeshipContractTypeEarningsEvent CreateRedundancyContractType(
            ApprenticeshipContractTypeEarningsEvent earningEvent)
        {
            switch (earningEvent)
            {
                case ApprenticeshipContractType1EarningEvent act1:
                    return mapper.Map<ApprenticeshipContractType1RedundancyEarningEvent>(act1);
                case ApprenticeshipContractType2EarningEvent act2:
                    return mapper.Map<ApprenticeshipContractType2RedundancyEarningEvent>(act2);
                default: return null;
            }
        }

        public FunctionalSkillEarningsEvent CreateRedundancyFunctionalSkillType(
            FunctionalSkillEarningsEvent functionalSkillEarning)
        {
            throw new NotImplementedException();
        }
    }
}