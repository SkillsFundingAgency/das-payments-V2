using System;
using Autofac;
using ESFA.DC.Logging.Enums;
using SFA.DAS.Payments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    bool.TryParse(configHelper.GetSettingOrDefault("ProcessMessageSequentially", "false"), out bool processMessageSequentially);

                    if (!TimeSpan.TryParse(configHelper.GetSettingOrDefault("DelayedMessageRetryDelay", "00:00:10"), out var delayedRetryDelay))
                        delayedRetryDelay = new TimeSpan(0, 0, 10);

                    return new ApplicationConfiguration
                    {
                        EndpointName = configHelper.GetSetting("EndpointName"),
                        StorageConnectionString = configHelper.GetConnectionString("StorageConnectionString"),
                        ServiceBusConnectionString = configHelper.GetConnectionString("ServiceBusConnectionString"),
                        FailedMessagesQueue = configHelper.GetSetting("FailedMessagesQueue"),
                        ProcessMessageSequentially = processMessageSequentially,
                        NServiceBusLicense = configHelper.GetSetting("DasNServiceBusLicenseKey"),
                        ImmediateMessageRetries = configHelper.GetSettingOrDefault("ImmediateMessageRetries", 1),
                        DelayedMessageRetries = configHelper.GetSettingOrDefault("DelayedMessageRetries", 3),
                        DelayedMessageRetryDelay = delayedRetryDelay,
                    };

                })
                .As<IApplicationConfiguration>()
                .SingleInstance();
        }
    }
}