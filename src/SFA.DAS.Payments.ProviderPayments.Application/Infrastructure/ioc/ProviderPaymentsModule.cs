using Autofac;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Domain;

namespace SFA.DAS.Payments.ProviderPayments.Application.Infrastructure.ioc
{
    public class ProviderPaymentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ValidatePaymentMessage>().AsImplementedInterfaces();
            builder.RegisterType<ProviderPaymentsRepository>().AsImplementedInterfaces();
            builder.RegisterType<MonthEndEventHandlerService>().AsImplementedInterfaces();
        }

    }
}
