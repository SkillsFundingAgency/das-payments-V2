﻿using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules
{
    public class MessagingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var config = c.Resolve<IApplicationConfiguration>();
                var endpointConfiguration = new EndpointConfiguration(config.EndpointName);

                var conventions = endpointConfiguration.Conventions();
                conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));
                conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Commands") ?? false));
                conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Events") ?? false));

                var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
                persistence.ConnectionString(config.StorageConnectionString);

                endpointConfiguration.DisableFeature<TimeoutManager>();
                var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                transport.ConnectionString(config.ServiceBusConnectionString);
                endpointConfiguration.SendFailedMessagesTo(config.FailedMessagesQueue);
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                endpointConfiguration.EnableInstallers();
                endpointConfiguration.UseContainer<AutofacBuilder>();
                return endpointConfiguration;
            })
            .As<EndpointConfiguration>()
            .InstancePerDependency();
        }
    }
}