using System;
using Autofac;
using ESFA.DC.Logging.Enums;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Configuration;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper >();
                    bool.TryParse(configHelper.GetSetting("ProcessMessageSequentially"), out bool processMessageSequentially);

                    return new ApplicationConfiguration
                    {
                        EndpointName = configHelper.GetSetting("EndpointName"),
                        StorageConnectionString = configHelper.GetConnectionString("StorageConnectionString"),
                        ServiceBusConnectionString = configHelper.GetConnectionString("ServiceBusConnectionString"),
                        FailedMessagesQueue = configHelper.GetSetting("FailedMessagesQueue"),
                        ProcessMessageSequentially = processMessageSequentially
                    };
                   
                })
                .As<IApplicationConfiguration>()
                .SingleInstance();
        }
    }
}