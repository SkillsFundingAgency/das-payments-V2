﻿using System;
using System.Linq;
using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Messaging.Telemetry;
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

                var endpointName = new EndpointName(config.EndpointName);
                EndpointConfigurationEvents.OnConfiguringEndpoint(endpointName);
                var endpointConfiguration = new EndpointConfiguration(endpointName.Name);
                EndpointConfigurationEvents.OnEndpointConfigured(endpointConfiguration);



                var conventions = endpointConfiguration.Conventions();
                conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));
                conventions.DefiningCommandsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Commands") ?? false));
                conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && ((type.Namespace?.Contains(".Messages.Events") ?? false) || (type.Namespace?.Contains(".Messages.Core") ?? false)));

                var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
                persistence.ConnectionString(config.StorageConnectionString);

                endpointConfiguration.DisableFeature<TimeoutManager>();
                var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                transport
                    .ConnectionString(config.ServiceBusConnectionString)
                    .Transactions(TransportTransactionMode.ReceiveOnly)
                    .RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
                builder.RegisterInstance(transport)
                    .As<TransportExtensions<AzureServiceBusTransport>>()
                    .SingleInstance();
                EndpointConfigurationEvents.OnConfiguringTransport(transport);  //TODO: find AutoFac & NSB way to do this
                endpointConfiguration.SendFailedMessagesTo(config.FailedMessagesQueue);
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                endpointConfiguration.EnableInstallers();
                endpointConfiguration.Pipeline.Register(typeof(TelemetryHandlerBehaviour), "Sends handler timing to telemetry service.");
                endpointConfiguration.EnableCallbacks(makesRequests: false);
                if (config.ProcessMessageSequentially) endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

                endpointConfiguration.Pipeline.Register(typeof(ExceptionHandlingBehavior), "Logs exceptions to the payments logger");
                return endpointConfiguration;
            })
            .As<EndpointConfiguration>()
            .SingleInstance();

            builder.RegisterType<TelemetryHandlerBehaviour>();
            builder.RegisterType<EndpointInstanceFactory>()
                .As<IEndpointInstanceFactory>()
                .SingleInstance();
            builder.RegisterType<ExceptionHandlingBehavior>()
                .SingleInstance();
        }
    }

    public class EndpointName
    {
        public EndpointName(string endpointName)
        {
            Name = endpointName;
        }

        public string Name { get; set; }
    }
}