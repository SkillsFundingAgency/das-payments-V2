using System.Linq;
using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.FundingSource.LevyTransactionService.Handlers;
using SFA.DAS.Payments.FundingSource.LevyTransactionService.Infrastructure.Messaging;
using SFA.DAS.Payments.Messaging.Serialization;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.FundingSource.LevyTransactionService.Infrastructure.Ioc
{
    public class LevyTransactionServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    var appConfig = c.Resolve<IApplicationConfiguration>();
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new StatelessServiceBusBatchCommunicationListener(
                        configHelper.GetConnectionString("ServiceBusConnectionString"),
                        appConfig.EndpointName,
                        appConfig.FailedMessagesQueue,
                        c.Resolve<IPaymentLogger>(),
                        c.Resolve<IContainerScopeFactory>(),
                        c.Resolve<ITelemetry>(),
                        c.Resolve<IMessageDeserializer>(),
                        c.Resolve<IApplicationMessageModifier>());
                })
                .As<IStatelessServiceBusBatchCommunicationListener>()
                .SingleInstance();

            builder.RegisterType<RecordLevyTransactionBatchHandler>()
                .As<IHandleMessageBatches<CalculatedRequiredLevyAmount>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<JobMessageClientFactory>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.Register((c, p) =>
                {
                    var logger = c.Resolve<IPaymentLogger>();
                    var endpointConfig = CreateEndpointConfiguration(c, logger);
                    return new MessageSessionFactory(endpointConfig);
                })
                .As<IMessageSessionFactory>()
                .SingleInstance();
        }

        private static EndpointConfiguration CreateEndpointConfiguration(IComponentContext container,
            IPaymentLogger logger)
        {
            var config = container.Resolve<IApplicationConfiguration>();
            var configHelper = container.Resolve<IConfigurationHelper>();
            var endpointConfiguration = new EndpointConfiguration(config.EndpointName);

            var conventions = endpointConfiguration.Conventions();
            conventions
                .DefiningCommandsAs(t => t.IsAssignableTo<JobsCommand>());

            var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            persistence.ConnectionString(config.StorageConnectionString);

            endpointConfiguration.DisableFeature<TimeoutManager>();
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport
                .ConnectionString(configHelper.GetConnectionString("ServiceBusConnectionString"))
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);

            endpointConfiguration.SendFailedMessagesTo(config.FailedMessagesQueue);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();

            endpointConfiguration.RegisterComponents(cfg => cfg.RegisterSingleton(logger));
            endpointConfiguration.SendOnly();
            return endpointConfiguration;
        }
    }
}