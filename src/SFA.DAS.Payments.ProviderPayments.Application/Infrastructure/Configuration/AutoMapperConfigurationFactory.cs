using AutoMapper;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Infrastructure.Configuration
{
    public static class AutoMapperConfigurationFactory
    {
        public static MapperConfiguration CreateMappingConfig()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FundingSourcePaymentEvent, ProviderPeriodicPayment>();
            });
        }
    }
}