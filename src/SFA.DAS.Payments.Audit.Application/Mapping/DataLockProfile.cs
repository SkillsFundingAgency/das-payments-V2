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
                .ForMember(x => x.AcademicYear, opt => opt.MapFrom(s => s.CollectionPeriod.AcademicYear))
                .ForMember(x => x.CollectionYear, opt => opt.MapFrom(s => s.CollectionPeriod.AcademicYear))
                .ForMember(x => x.CollectionPeriod, opt => opt.MapFrom(s => s.CollectionPeriod.Period))
                .ForMember(x => x.Earnings, opt => opt.Ignore())
                .ForMember(x => x.ContractType, opt => opt.Ignore());


            CreateMap<PayableEarningEvent, DataLockEventModel>()
                .ForMember(x => x.IsPayable, opt => opt.UseValue(true));

            CreateMap<EarningFailedDataLockMatching, DataLockEventModel>()
                .ForMember(x => x.IsPayable, opt => opt.UseValue(false));
            
            CreateMap<FunctionalSkillDataLockEvent, DataLockEventModel>()
                .Include<PayableFunctionalSkillEarningEvent, DataLockEventModel>()
                .Include<FunctionalSkillEarningFailedDataLockMatching, DataLockEventModel>()
                .ForMember(x => x.AcademicYear, opt => opt.MapFrom(s => s.CollectionPeriod.AcademicYear))
                .ForMember(x => x.CollectionYear, opt => opt.MapFrom(s => s.CollectionPeriod.AcademicYear))
                .ForMember(x => x.CollectionPeriod, opt => opt.MapFrom(s => s.CollectionPeriod.Period))
                .ForMember(x => x.AgreementId, opt => opt.Ignore())
                .ForMember(x => x.OnProgrammeEarnings, opt => opt.Ignore())
                .ForMember(x => x.IncentiveEarnings, opt => opt.Ignore());

            CreateMap<PayableFunctionalSkillEarningEvent, DataLockEventModel>()
                .ForMember(x => x.IsPayable, opt => opt.UseValue(true));

            CreateMap<FunctionalSkillEarningFailedDataLockMatching, DataLockEventModel>()
                .ForMember(x => x.IsPayable, opt => opt.UseValue(false));
        }
    }
}
