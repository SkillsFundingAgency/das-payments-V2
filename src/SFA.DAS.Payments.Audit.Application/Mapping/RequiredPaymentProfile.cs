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
                .Include<ApprenticeshipContractType1RequiredPaymentEvent, RequiredPaymentEventModel>()
                .MapPeriodisedCommon()
                .ForMember(dest => dest.EarningEventId, opt => opt.MapFrom(source => source.EarningEventId))
                ;

            CreateMap<IncentiveRequiredPaymentEvent, RequiredPaymentEventModel>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.Type))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.UseValue(1M))
                ;

            CreateMap<ApprenticeshipContractTypeRequiredPaymentEvent, RequiredPaymentEventModel>()
                .Include<ApprenticeshipContractType2RequiredPaymentEvent, RequiredPaymentEventModel>()
                .Include<ApprenticeshipContractType1RequiredPaymentEvent, RequiredPaymentEventModel>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType) source.OnProgrammeEarningType))
                ;

            CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, RequiredPaymentEventModel>()
                .ForMember(dest => dest.ContractType, opt => opt.UseValue(ContractType.Act2))
                ;

            CreateMap<ApprenticeshipContractType1RequiredPaymentEvent, RequiredPaymentEventModel>()
                .ForMember(dest => dest.ContractType, opt => opt.UseValue(ContractType.Act1))
                .ForMember(dest => dest.AgreementId, opt => opt.MapFrom(source => source.AgreementId))
                ;

        }
    }
}