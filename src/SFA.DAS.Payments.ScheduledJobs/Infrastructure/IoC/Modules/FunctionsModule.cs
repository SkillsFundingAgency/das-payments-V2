using Autofac;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;
using SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules
{
    public class FunctionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FunctionsConfigurationHelper>().As<IConfigurationHelper>().SingleInstance();
            builder.RegisterType<AuditDataCleanUpService>().As<IAuditDataCleanUpService>().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipDataService>().As<IApprenticeshipsDataService>().SingleInstance();
        }
    }
}
