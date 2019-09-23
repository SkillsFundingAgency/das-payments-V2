using Autofac;
using SFA.DAS.Payments.Monitoring.Jobs.Application;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Infrastructure.Ioc
{
    public class JobServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobStorageService>()
                .As<IJobStorageService>()
                .InstancePerLifetimeScope();
        }
    }
}