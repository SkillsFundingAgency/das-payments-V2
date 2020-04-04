using System;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents
{
    public interface IEarningEventMapper
    {
        EarningEventModel Map(EarningEvent earningEvent);
    }

    public class EarningEventMapper : IEarningEventMapper
    {
        private readonly IMapper mapper;

        public EarningEventMapper(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public EarningEventModel Map(EarningEvent earningEvent)
        {
            return mapper.Map<EarningEventModel>(earningEvent);
        }
    }
}