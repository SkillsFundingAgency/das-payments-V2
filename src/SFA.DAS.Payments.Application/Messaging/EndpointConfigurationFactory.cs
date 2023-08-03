using System.Linq;
using System.Net;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Messaging
{
    //TODO: Temp hack to get SF working with the new NSB config API.  Will refactor once working.
    public class EndpointConfigurationFactory
    {
        public static EndpointConfiguration Create(IApplicationConfiguration config)
        {
            var endpointName = new EndpointName(config.EndpointName);
            EndpointConfigurationEvents.OnConfiguringEndpoint(endpointName);
            var endpointConfiguration = new EndpointConfiguration(endpointName.Name);
            EndpointConfigurationEvents.OnEndpointConfigured(endpointConfiguration);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));
            conventions.DefiningCommandsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Commands") ?? false));
            conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && ((type.Namespace?.Contains(".Messages.Events") ?? false) || (type.Namespace?.Contains(".Messages.Core") ?? false)));

            var persistence = endpointConfiguration.UsePersistence<AzureTablePersistence>();
            persistence.ConnectionString(config.StorageConnectionString);


            //endpointConfiguration.DisableFeature<TimeoutManager>();
            if (!string.IsNullOrEmpty(config.NServiceBusLicense))
            {
                var license = WebUtility.HtmlDecode(config.NServiceBusLicense);
                endpointConfiguration.License(license);
            }

            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport
                .ConnectionString(config.ServiceBusConnectionString)
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .SubscriptionRuleNamingConvention(messageType =>
                    messageType.FullName?.Split('.').LastOrDefault() ?? messageType.Name);
            //.RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
            transport.PrefetchCount(20);
            //builder.RegisterInstance(transport)
            //    .As<TransportExtensions<AzureServiceBusTransport>>()
            //    .SingleInstance();
            EndpointConfigurationEvents.OnConfiguringTransport(transport);  //TODO: find AutoFac & NSB way to do this
            endpointConfiguration.SendFailedMessagesTo(config.FailedMessagesQueue);
            endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
            endpointConfiguration.EnableInstallers();
            //endpointConfiguration.Pipeline.Register(typeof(TelemetryHandlerBehaviour), "Sends handler timing to telemetry service.");
            endpointConfiguration.EnableCallbacks(makesRequests: false);
            if (config.ProcessMessageSequentially) endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

            endpointConfiguration.Pipeline.Register(typeof(ExceptionHandlingBehavior), "Logs exceptions to the payments logger");

            var recoverability = endpointConfiguration.Recoverability();
            recoverability.Immediate(immediate => immediate.NumberOfRetries(config.ImmediateMessageRetries));
            recoverability.Delayed(delayed =>
            {
                delayed.NumberOfRetries(config.DelayedMessageRetries);
                delayed.TimeIncrease(config.DelayedMessageRetryDelay);
            });

            return endpointConfiguration;

        }
    }

}

