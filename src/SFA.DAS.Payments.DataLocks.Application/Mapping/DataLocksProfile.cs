﻿using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.Application.Mapping
{
    public class DataLocksProfile : Profile
    {
        public DataLocksProfile()
        {
            CreateMap<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>()
                .ForMember(destinationMember => destinationMember.EarningEventId,  opt => opt.MapFrom(source => source.EventId))
                .ForMember(destinationMember => destinationMember.EventId, opt => opt.Ignore());

            CreateMap<ApprenticeshipContractType1EarningEvent, EarningFailedDataLockMatching>()
                .ForMember(destinationMember => destinationMember.EarningEventId,opt => opt.MapFrom(source => source.EventId))
                .ForMember(destinationMember => destinationMember.EventId, opt => opt.Ignore());
        }
    }
}
