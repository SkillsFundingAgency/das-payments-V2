using AutoMapper;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.Mapping
{
    public class RequiredPaymentProfile : Profile
    {
        public RequiredPaymentProfile()
        {
            CreateMap<RequiredPaymentEvent, RequiredPaymentEventModel>()
                .Include<IncentiveRequiredPaymentEvent, RequiredPaymentEventModel>()
                .Include<ApprenticeshipContractType2RequiredPaymentEvent, RequiredPaymentEventModel>()
                .MapPeriodisedCommon()
                .ForMember(dest => dest.EarningEventId, opt => opt.MapFrom(source => source.EarningEventId))
                .ForMember(dest => dest.ContractType, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionType, opt => opt.Ignore())
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.Ignore())
                ;

            CreateMap<IncentiveRequiredPaymentEvent, RequiredPaymentEventModel>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.Type))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.UseValue(1M));


            CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, RequiredPaymentEventModel>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.OnProgrammeEarningType))
                .ForMember(dest => dest.ContractType, opt => opt.UseValue(ContractType.Act2))
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.MapFrom(source => source.SfaContributionPercentage));
        }
    }
}