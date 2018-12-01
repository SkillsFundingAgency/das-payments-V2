using AutoMapper;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.Mapping.FundingSource
{
    public class FundingSourceProfile: Profile
    {
        public FundingSourceProfile()
        {
            CreateMap<FundingSourcePaymentEvent, FundingSourceEventModel>()
                //.ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => source.tr))
                .ForMember(dest => dest.FundingSource, opt => opt.MapFrom(source => source.FundingSourceType))
                .ForMember(dest => dest.RequiredPaymentEventId, opt => opt.MapFrom(source => source.RequiredPaymentEventId))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                ;
        }
    }
}