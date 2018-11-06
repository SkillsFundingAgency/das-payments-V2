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

                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, SfaCoInvestedFundingSourcePaymentEvent>()
                  .ForMember(dest => dest.ContractType, opt => opt.UseValue<byte>(1))
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.CoInvestedSfa));

                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, EmployerCoInvestedFundingSourcePaymentEvent>()
                 .ForMember(dest => dest.ContractType, opt => opt.UseValue<byte>(2))
                 .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.CoInvestedEmployer));
            });
        }
    }
}