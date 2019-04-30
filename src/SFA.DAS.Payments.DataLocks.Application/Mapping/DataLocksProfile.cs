using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.Application.Mapping
{
    public class DataLocksProfile : Profile
    {
        public DataLocksProfile()
        {
            CreateMap<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>()
                .ForMember(destinationMember => destinationMember.EarningEventId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(destinationMember => destinationMember.EventId, opt => opt.Ignore())
                .ForMember(destinationMember => destinationMember.Priority, opt => opt.Ignore())
                .ForMember(destinationMember => destinationMember.AccountId, opt => opt.Ignore())
                .ForMember(destinationMember => destinationMember.CommitmentId, opt => opt.Ignore())
                .ForMember(destinationMember => destinationMember.CommitmentVersion, opt => opt.Ignore());

            CreateMap<ApprenticeshipContractType1EarningEvent, NonPayableEarningEvent>()
                .ForMember(destinationMember => destinationMember.EarningEventId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(destinationMember => destinationMember.EventId, opt => opt.Ignore())
                .ForMember(x => x.Errors, opt => opt.Ignore());
        }
    }
}
