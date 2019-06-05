using System.Linq;
using Autofac;
using Autofac.Core;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Messaging.Telemetry;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Infrastructure.Ioc.Modules
{
    public class DasMessagingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var config = c.Resolve<IApplicationConfiguration>();
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    var endpointConfiguration = new EndpointConfiguration(config.EndpointName);

                    var conventions = endpointConfiguration.Conventions();
                    conventions.DefiningMessagesAs(type =>
                        (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) &&
                        (type.Namespace?.Contains(".Messages") ?? false));
                    conventions.DefiningCommandsAs(type =>
                        (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) &&
                        (type.Namespace?.Contains(".Messages.Commands") ?? false));
                    conventions.DefiningEventsAs(type =>
                        (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) &&
                        (type.Namespace?.Contains(".Messages.Events") ?? false));

                    var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
                    persistence.ConnectionString(config.StorageConnectionString);

                    endpointConfiguration.DisableFeature<TimeoutManager>();
                    var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                    transport
                        .ConnectionString(configHelper.GetConnectionString("DASServiceBusConnectionString"))
                        .Transactions(TransportTransactionMode.ReceiveOnly)
                        .RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
                    builder.RegisterInstance(transport)
                        .As<TransportExtensions<AzureServiceBusTransport>>()
                        .SingleInstance();
                    EndpointConfigurationEvents
                        .OnConfiguringTransport(transport); //TODO: find AutoFac & NSB way to do this
                    endpointConfiguration.SendFailedMessagesTo(config.FailedMessagesQueue);
                    endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                    endpointConfiguration.EnableInstallers();
                    endpointConfiguration.Pipeline.Register(typeof(TelemetryHandlerBehaviour),
                        "Sends handler timing to telemetry service.");
                    endpointConfiguration.EnableCallbacks(makesRequests: false);

                    if (config.ProcessMessageSequentially) endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

                    endpointConfiguration.Pipeline.Register(typeof(ExceptionHandlingBehavior),
                        "Logs exceptions to the payments logger");
                    return endpointConfiguration;
                })
                .Named<EndpointConfiguration>("DASEndpointConfiguration")
                .SingleInstance();
            

            builder.RegisterType<DasStatelessEndpointCommunicationListener>()
                .WithParameter(new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(EndpointConfiguration),
                    (pi, ctx) => ctx.ResolveNamed<EndpointConfiguration>("DASEndpointConfiguration")))
                .As<IDasStatelessEndpointCommunicationListener>()
                .SingleInstance();
        }
    }
}