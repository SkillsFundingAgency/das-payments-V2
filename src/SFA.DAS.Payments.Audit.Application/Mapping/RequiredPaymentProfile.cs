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
            CreateMap<PeriodisedRequiredPaymentEvent, RequiredPaymentEventModel>()
                .Include<CalculatedRequiredIncentiveAmount, RequiredPaymentEventModel>()
                .Include<CalculatedRequiredCoInvestedAmount, RequiredPaymentEventModel>()
                .Include<CalculatedRequiredLevyAmount, RequiredPaymentEventModel>()
                .Include<CompletionPaymentHeldBackEvent, RequiredPaymentEventModel>()
                .MapPeriodisedCommon()
                .ForMember(dest => dest.EarningEventId, opt => opt.MapFrom(source => source.EarningEventId))
                .ForMember(dest => dest.ApprenticeshipEmployerType, opt => opt.MapFrom(source => source.ApprenticeshipEmployerType))
                ;

            CreateMap<CalculatedRequiredIncentiveAmount, RequiredPaymentEventModel>()
                  .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.Type))
                  .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                  .ForMember(dest => dest.SfaContributionPercentage, opt => opt.UseValue(1M))
                  ;

            CreateMap<CalculatedRequiredOnProgrammeAmount, RequiredPaymentEventModel>()
                .Include<CalculatedRequiredCoInvestedAmount, RequiredPaymentEventModel>()
                .Include<CalculatedRequiredLevyAmount, RequiredPaymentEventModel>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.OnProgrammeEarningType))
                ;

            CreateMap<CalculatedRequiredCoInvestedAmount, RequiredPaymentEventModel>()
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                ;

            CreateMap<CalculatedRequiredLevyAmount, RequiredPaymentEventModel>()
                .ForMember(dest => dest.ContractType, opt => opt.UseValue(ContractType.Act1))
                .ForMember(dest => dest.AgreementId, opt => opt.MapFrom(source => source.AgreementId))
                ;

            CreateMap<CompletionPaymentHeldBackEvent, RequiredPaymentEventModel>()
                .ForMember(dest => dest.NonPaymentReason, opt => opt.UseValue(NonPaymentReason.InsufficientEmployerContribution))
                ;
        }
    }
}