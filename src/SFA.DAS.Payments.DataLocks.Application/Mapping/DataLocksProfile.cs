using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.Application.Mapping
{
    public class DataLocksProfile : Profile
    {
        public DataLocksProfile()
        {
            CreateMap<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>();

            CreateMap<ApprenticeshipContractType1EarningEvent, NonPayableEarningEvent>()
                .ForMember(x => x.Errors, opt => opt.Ignore());
        }
    }
}
