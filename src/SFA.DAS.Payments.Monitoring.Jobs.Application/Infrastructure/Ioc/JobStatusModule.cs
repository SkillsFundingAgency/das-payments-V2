using Autofac;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Data;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Infrastructure.Ioc
{
    public class JobStatusModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new JobStatusDataContext(configHelper.GetConnectionString("PaymentsConnectionString"));
                })
                .As<IJobStatusDataContext>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ProviderEarningsJobService>()
                .As<IProviderEarningsJobService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<JobStatusService>()
                .As<IJobStatusService>()
                .InstancePerLifetimeScope();
        }
    }
}