using System;
using NServiceBus;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class EndpointConfigurationEvents
    {
        public static event EventHandler<TransportExtensions<AzureServiceBusTransport>> ConfiguringTransport;

        public static void OnConfiguringTransport(TransportExtensions<AzureServiceBusTransport> transportConfiguration)
        {
            ConfiguringTransport?.Invoke(new object(), transportConfiguration);
        }
    }
}