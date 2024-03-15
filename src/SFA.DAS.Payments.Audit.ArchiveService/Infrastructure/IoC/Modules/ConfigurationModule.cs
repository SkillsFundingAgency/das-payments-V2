using Autofac;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC.Modules
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new PeriodEndArchiveConfiguration
                    {
                        ResourceGroup = configHelper.GetSetting("ResourceGroup"),
                        AzureDataFactoryName = configHelper.GetSetting("AzureDataFactoryName"),
                        PipeLine = configHelper.GetSetting("PipeLine"),
                        SubscriptionId = configHelper.GetSetting("SubscriptionId"),
                        TenantId = configHelper.GetSetting("TenantId"),
                        ApplicationId = configHelper.GetSetting("ApplicationId"),
                        AuthenticationKey = configHelper.GetSetting("AuthenticationKey"),
                        SleepDelay = int.Parse(configHelper.GetSetting("SleepDelay")),
                        AuthorityUri = configHelper.GetSetting("AuthorityUri"),
                        ManagementUri = configHelper.GetSetting("ManagementUri")
                    };
                })
                .As<IPeriodEndArchiveConfiguration>().SingleInstance();
            builder.RegisterType<FunctionsConfigurationHelper>().As<IConfigurationHelper>().SingleInstance();
        }
    }
}