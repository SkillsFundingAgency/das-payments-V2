using Autofac;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new ScheduledJobsConfiguration()
                    {
                        EndpointName = configHelper.GetSetting("EndpointName"),
                        ServiceBusConnectionString = configHelper.GetConnectionString("ServiceBusConnectionString"),
                        DasNServiceBusLicenseKey = configHelper.GetSetting("DasNServiceBusLicenseKey"),
                        LevyAccountBalanceEndpoint = configHelper.GetSetting("LevyAccountBalanceEndpoint")
                    };

                })
                .As<IScheduledJobsConfiguration>()
                .SingleInstance();
        }
    }
}