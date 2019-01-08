using Autofac;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.JobsService.JobStatus;

namespace SFA.DAS.Payments.Monitoring.JobsService.Infrastructure.Ioc
{
    public class JobStatusServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobsStatusServiceFacade>()
                .As<IJobsStatusServiceFacade>()
                .InstancePerLifetimeScope();

        }
    }
}