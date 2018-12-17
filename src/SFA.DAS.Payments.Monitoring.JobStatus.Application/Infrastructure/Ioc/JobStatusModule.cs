using Autofac;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Infrastructure.Ioc
{
    public class JobStatusModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProviderEarningsJobService>()
                .As<IProviderEarningsJobService>()
                .InstancePerLifetimeScope();
        }
    }
}