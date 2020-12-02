using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Cache;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Infrastructure.Ioc
{
    public class ProviderPaymentsInstaller: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DasEndpointFactory>()
                   .As<IDasEndpointFactory>()
                   .SingleInstance();

            builder.RegisterType<IlrSubmissionCache>()
                   .As<IDataCache<ReceivedProviderEarningsEvent>>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<MonthEndCache>()
                .As<IMonthEndCache>()
                .InstancePerLifetimeScope();
            builder.RegisterBuildCallback(c =>
            {
                var recoverability = c.Resolve<EndpointConfiguration>()
                    .Recoverability();
                recoverability.Immediate(immediate => immediate.NumberOfRetries(3));
            });
        }
    }
}