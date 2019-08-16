using AutoMapper;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.Mapping
{
    public class DataLockProfile : Profile
    {
        public DataLockProfile()
        {
            CreateMap<DataLockEvent, DataLockEventModel>()
                .Include<PayableEarningEvent, DataLockEventModel>()
                .Include<EarningFailedDataLockMatching, DataLockEventModel>()
                ;

            CreateMap<PayableEarningEvent, DataLockEventModel>()
                .ForMember(x => x.IsPayable, opt => opt.UseValue(true))
                ;

            CreateMap<EarningFailedDataLockMatching, DataLockEventModel>()
                .ForMember(x => x.IsPayable, opt => opt.UseValue(false))
                ;
        }
    }
}
