
using Autofac;
using SFA.DAS.Payments.ProviderAdjustments.Application.Repositories;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.Application.Infrastructure.ioc
{
    public class ProviderPaymentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProviderAdjustmentRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ProviderAdjustmentCalculator>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ProviderAdjustmentsProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BulkWriterProviderAdjustmentsConfiguration>().AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
