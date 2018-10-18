using Autofac;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.Application.Infrastructure.ioc
{
    public class ProviderPaymentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProviderPaymentsRepository>().AsImplementedInterfaces();
            builder.RegisterType<FundingSourceEventHandlerService>().AsImplementedInterfaces();
        }

    }
}
