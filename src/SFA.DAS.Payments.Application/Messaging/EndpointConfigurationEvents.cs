using System;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class EndpointConfigurationEvents
    {
        public static event EventHandler<TransportExtensions<AzureServiceBusTransport>> ConfiguringTransport;
        public static event EventHandler<EndpointName> ConfiguringEndpointName;
        public static event EventHandler<EndpointConfiguration> EndpointConfigured;

        public static void OnConfiguringTransport(TransportExtensions<AzureServiceBusTransport> transportConfiguration)
        {
            ConfiguringTransport?.Invoke(new object(), transportConfiguration);
        }

        public static void OnConfiguringEndpoint(EndpointName endpointConfiguration)
        {
            ConfiguringEndpointName?.Invoke(new object(), endpointConfiguration);
        }

        public static void OnEndpointConfigured(EndpointConfiguration endpointConfiguration)
        {
            EndpointConfigured?.Invoke(new object(), endpointConfiguration);
        }
    }
}