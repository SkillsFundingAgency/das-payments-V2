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
                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, RequiredCoInvestedPayment>();

                cfg.CreateMap<RequiredPaymentEvent, FundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType2RequiredPaymentEvent, SfaCoInvestedFundingSourcePaymentEvent>()
                    .Include<ApprenticeshipContractType2RequiredPaymentEvent, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .Include<IncentiveRequiredPaymentEvent, SfaFullyFundedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.EventId, opt => opt.Ignore())
                    .ForMember(dest => dest.EventTime, opt => opt.Ignore())
                    .ForMember(dest => dest.RequiredPaymentEventId, opt => opt.MapFrom(source => source.EventId));

                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, SfaCoInvestedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.OnProgrammeEarningType))
                    .ForMember(dest => dest.ContractType, opt => opt.UseValue<ContractType>(ContractType.Act2))
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.CoInvestedSfa));

                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.OnProgrammeEarningType))
                    .ForMember(dest => dest.ContractType, opt => opt.UseValue<ContractType>(ContractType.Act2))
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.CoInvestedEmployer));

                cfg.CreateMap<IncentiveRequiredPaymentEvent, SfaFullyFundedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                    .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.Type))
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.FullyFundedSfa));
            });
        }
    }
}