using AutoMapper;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.Audit.Application.Mapping
{
    public class FundingSourceProfile : Profile
    {
        public FundingSourceProfile()
        {
            CreateMap<FundingSourcePaymentEvent, FundingSourceEventModel>()
                .Include<SfaCoInvestedFundingSourcePaymentEvent, FundingSourceEventModel>()
                .Include<EmployerCoInvestedFundingSourcePaymentEvent, FundingSourceEventModel>()
                .Include<SfaFullyFundedFundingSourcePaymentEvent, FundingSourceEventModel>()
                .Include<LevyFundingSourcePaymentEvent, FundingSourceEventModel>()
                .MapPeriodisedCommon()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => source.TransactionType))
                .ForMember(dest => dest.FundingSource, opt => opt.MapFrom(source => source.FundingSourceType))
                .ForMember(dest => dest.RequiredPaymentEventId, opt => opt.MapFrom(source => source.RequiredPaymentEventId))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                ;
            CreateMap<SfaCoInvestedFundingSourcePaymentEvent, FundingSourceEventModel>();
            CreateMap<EmployerCoInvestedFundingSourcePaymentEvent, FundingSourceEventModel>();
            CreateMap<SfaFullyFundedFundingSourcePaymentEvent, FundingSourceEventModel>();
        }
    }
}