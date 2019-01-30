using AutoMapper;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration
{
    public static class AutoMapperConfigurationFactory
    {
        public static MapperConfiguration CreateMappingConfig()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RequiredPaymentEvent, FundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractTypeRequiredPaymentEvent, FundingSourcePaymentEvent>()
                    .Include<IncentiveRequiredPaymentEvent, SfaFullyFundedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.EventId, opt => opt.Ignore())
                    .ForMember(dest => dest.EventTime, opt => opt.Ignore())
                    .ForMember(dest => dest.RequiredPaymentEventId, opt => opt.MapFrom(source => source.EventId));

                cfg.CreateMap<ApprenticeshipContractType1RequiredPaymentEvent, EmployerCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<ApprenticeshipContractType1RequiredPaymentEvent, SfaCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<ApprenticeshipContractType1RequiredPaymentEvent, SfaFullyFundedFundingSourcePaymentEvent>();

                cfg.CreateMap<ApprenticeshipContractType1RequiredPaymentEvent, LevyFundingSourcePaymentEvent>()
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.AgreementId, opt => opt.MapFrom(requiredPayment => requiredPayment.AgreementId));

                cfg.CreateMap<ApprenticeshipContractType1RequiredPaymentEvent, FundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType1RequiredPaymentEvent, LevyFundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType1RequiredPaymentEvent, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType1RequiredPaymentEvent, SfaCoInvestedFundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType1RequiredPaymentEvent, SfaFullyFundedFundingSourcePaymentEvent>()
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.ContractType, opt => opt.UseValue(ContractType.Act1))
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.AmountDue, opt => opt.Ignore())
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.OnProgrammeEarningType));

                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, EmployerCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, SfaCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, SfaFullyFundedFundingSourcePaymentEvent>();

                cfg.CreateMap<ApprenticeshipContractTypeRequiredPaymentEvent, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType2RequiredPaymentEvent, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.CoInvestedEmployer));

                cfg.CreateMap<ApprenticeshipContractTypeRequiredPaymentEvent, SfaCoInvestedFundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType2RequiredPaymentEvent, SfaCoInvestedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.CoInvestedSfa));

                cfg.CreateMap<ApprenticeshipContractTypeRequiredPaymentEvent, SfaFullyFundedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.FullyFundedSfa));

                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, FundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType2RequiredPaymentEvent, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType2RequiredPaymentEvent, SfaCoInvestedFundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType2RequiredPaymentEvent, SfaFullyFundedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.ContractType, opt => opt.UseValue(ContractType.Act2));

                cfg.CreateMap<ApprenticeshipContractTypeRequiredPaymentEvent, FundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType1RequiredPaymentEvent, FundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType2RequiredPaymentEvent, FundingSourcePaymentEvent>()
                    .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.OnProgrammeEarningType));

                cfg.CreateMap<IncentiveRequiredPaymentEvent, SfaFullyFundedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                    .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.Type))
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.FullyFundedSfa));

                cfg.CreateMap<ApprenticeshipContractType1RequiredPaymentEvent, RequiredLevyPayment>();
                cfg.CreateMap<EmployerCoInvestedPayment, EmployerCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<SfaCoInvestedPayment, SfaCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<LevyPayment, LevyFundingSourcePaymentEvent>();

                cfg.CreateMap<FundingSourcePayment, FundingSourcePaymentEvent>()
                    .Include<EmployerCoInvestedPayment, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .Include<SfaCoInvestedPayment, SfaCoInvestedFundingSourcePaymentEvent>()
                    .Include<LevyPayment, LevyFundingSourcePaymentEvent>()
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.FundingSourceType, opt => opt.MapFrom(payment => payment.Type))
                    ;
            });
        }
    }
}