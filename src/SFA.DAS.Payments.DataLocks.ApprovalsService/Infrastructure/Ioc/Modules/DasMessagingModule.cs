using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Messaging.Telemetry;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Infrastructure.Ioc.Modules
{
    public class DasMessagingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.Register((c, p) =>
            //    {
            //        var config = c.Resolve<IApplicationConfiguration>();
            //        var configHelper = c.Resolve<IConfigurationHelper>();
            //        var endpointConfiguration = new EndpointConfiguration(config.EndpointName);

            //        var conventions = endpointConfiguration.Conventions();
            //        conventions
            //            .DefiningMessagesAs(t => (t.IsInNamespace("SFA.DAS.CommitmentsV2.Messages") && t.Name.EndsWith("Event",StringComparison.OrdinalIgnoreCase))  || t.IsAssignableTo<IPaymentsMessage>())
            //            .DefiningEventsAs(t => t.IsInNamespace("SFA.DAS.CommitmentsV2.Messages.Events"));

            //        var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            //        persistence.ConnectionString(config.StorageConnectionString);

            //        endpointConfiguration.DisableFeature<TimeoutManager>();
            //        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            //        transport
            //            .ConnectionString(configHelper.GetConnectionString("DASServiceBusConnectionString"))
            //            .Transactions(TransportTransactionMode.ReceiveOnly)
            //            .RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
            //        builder.RegisterInstance(transport)
            //            .Named<TransportExtensions<AzureServiceBusTransport>>("DasTransportConfig")
            //            .SingleInstance();
            //        EndpointConfigurationEvents
            //            .OnConfiguringTransport(transport); //TODO: find AutoFac & NSB way to do this
            //        endpointConfiguration.SendFailedMessagesTo(config.FailedMessagesQueue);
            //        endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            //        endpointConfiguration.EnableInstallers();
            //        endpointConfiguration.Pipeline.Register(typeof(TelemetryHandlerBehaviour),
            //            "Sends handler timing to telemetry service.");
            //        endpointConfiguration.EnableCallbacks(makesRequests: false);

            //        if (config.ProcessMessageSequentially) endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

            //        endpointConfiguration.Pipeline.Register(typeof(ExceptionHandlingBehavior),
            //            "Logs exceptions to the payments logger");
            //        return endpointConfiguration;
            //    })
            //    .Named<EndpointConfiguration>("DASEndpointConfiguration")
            //    .SingleInstance();
            

            builder.RegisterType<DasStatelessEndpointCommunicationListener>()
                .WithParameter(new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(EndpointConfiguration),
                    (pi, ctx) => ctx.ResolveNamed<EndpointConfiguration>("DASEndpointConfiguration")))
                .As<IDasStatelessEndpointCommunicationListener>()
                .SingleInstance();
        }
    }
}