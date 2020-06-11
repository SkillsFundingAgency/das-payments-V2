using Autofac;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ScheduledJobs.ApprenticeshipsReferenceDataComparison;
using SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules
{
    public class FunctionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FunctionsConfigurationHelper>().As<IConfigurationHelper>().SingleInstance();
            builder.RegisterType<AuditDataCleanUpService>().As<IAuditDataCleanUpService>().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipsReferenceDataComparisonService>().As<IApprenticeshipsReferenceDataComparisonService>().SingleInstance();
        }
    }
}
