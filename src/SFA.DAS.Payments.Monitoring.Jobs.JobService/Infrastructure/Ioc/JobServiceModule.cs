using Autofac;
using SFA.DAS.Payments.Monitoring.Jobs.Application;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Infrastructure.Ioc
{
    public class JobServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobStorageService>().As<IJobStorageService>().InstancePerLifetimeScope();
            builder.RegisterType<CompletedMessageRepository>().As<ICompletedMessageRepository>().InstancePerLifetimeScope();
            builder.RegisterType<InProgressMessageRepository>().As<IInProgressMessageRepository>().InstancePerLifetimeScope();
            builder.RegisterType<JobModelRepository>().As<IJobModelRepository>().InstancePerLifetimeScope();
            builder.RegisterType<JobStatusRepository>().As<IJobStatusRepository>().InstancePerLifetimeScope();
        }
    }
}