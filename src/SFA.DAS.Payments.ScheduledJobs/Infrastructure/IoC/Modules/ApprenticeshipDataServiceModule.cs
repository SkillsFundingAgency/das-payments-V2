using Autofac;
using SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules
{
    public class ApprenticeshipDataServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CommitmentsApiClient>().As<ICommitmentsApiClient>().SingleInstance();
            builder.RegisterType<ApprenticeshipDataService>().As<IApprenticeshipsDataService>().SingleInstance();
        }
    }
}