using AutoMapper;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration
{
    public class AutoMapperConfigurationFactory
    {
        public static MapperConfiguration CreateMappingConfig()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, RequiredCoInvestedPayment>();
                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, CoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, SfaCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<ApprenticeshipContractType2RequiredPaymentEvent, EmployerCoInvestedFundingSourcePaymentEvent>();
            });
        }
    }
}