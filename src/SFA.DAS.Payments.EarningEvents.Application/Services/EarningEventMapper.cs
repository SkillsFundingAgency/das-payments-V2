using System.Collections.Generic;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Services
{
    public class EarningEventMapper : IEarningEventMapper
    {
        private readonly IMapper mapper;

        public EarningEventMapper(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public IEnumerable<ApprenticeshipContractType2EarningEvent> MapEarningEvent(FM36Global fm36Output)
        {
            return mapper.Map<IEnumerable<ApprenticeshipContractType2EarningEvent>>(fm36Output);
        }
    }
}