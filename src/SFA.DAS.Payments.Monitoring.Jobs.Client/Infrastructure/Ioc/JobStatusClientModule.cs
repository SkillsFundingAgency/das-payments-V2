using System.Linq;
using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure.Ioc
{
    public class JobStatusClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var logger = c.Resolve<IPaymentLogger>();
                    var endpointConfig = CreateEndpointConfiguration(c, logger);
                    return new MonitoringMessageSessionFactory(endpointConfig);
                })
                .As<IMonitoringMessageSessionFactory>()
                .SingleInstance();

            builder.Register((c, p) =>
                {
                    var logger = c.Resolve<IPaymentLogger>();
                    var factory = c.Resolve<IMonitoringMessageSessionFactory>();
                    return new EarningsJobClient(factory.Create(),logger);
                })
                .As<IEarningsJobClient>()
                .SingleInstance();

            builder.RegisterType<EarningsJobClientFactory>()
                .As<IEarningsJobClientFactory>()
                .SingleInstance();

            builder.RegisterType<JobStatusIncomingMessageBehaviour>()
                .SingleInstance();

            builder.RegisterType<JobStatusOutgoingMessageBehaviour>()
                .SingleInstance();

            builder.RegisterBuildCallback(c =>
            {
                c.Resolve<EndpointConfiguration>().Pipeline.Register(typeof(JobStatusIncomingMessageBehaviour),
                    "Job Status Incoming message behaviour");
                c.Resolve<EndpointConfiguration>().Pipeline.Register(typeof(JobStatusOutgoingMessageBehaviour),
                    "Job Status Outgoing message behaviour");
            });
        }

        private void Errors_MessageSentToErrorQueue(object sender, NServiceBus.Faults.FailedMessage e)
        {
            //TODO: get the message Id from the serialized message body and then use the JobClient to notify the jobs service of the failure
        }

        private EndpointConfiguration CreateEndpointConfiguration(IComponentContext container, IPaymentLogger logger)
        {
            var config = container.Resolve<IApplicationConfiguration>();
            var configHelper = container.Resolve<IConfigurationHelper>();
            var endpointConfiguration = new EndpointConfiguration(config.EndpointName);
            var jobsEndpointName = configHelper.GetSettingOrDefault("Monitoring_JobsService_EndpointName", "sfa-das-payments-monitoring-jobs");

            var conventions = endpointConfiguration.Conventions();
            conventions
                .DefiningCommandsAs(t => t.IsAssignableTo<JobsCommand>());

            var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            persistence.ConnectionString(config.StorageConnectionString);

            endpointConfiguration.DisableFeature<TimeoutManager>();
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport
                .ConnectionString(configHelper.GetConnectionString("MonitoringServiceBusConnectionString"))
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);

            transport.Routing().RouteToEndpoint(typeof(RecordStartedProcessingEarningsJob).Assembly, jobsEndpointName);
            endpointConfiguration.SendFailedMessagesTo(config.FailedMessagesQueue);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();

            endpointConfiguration.RegisterComponents(cfg => cfg.RegisterSingleton((IPaymentLogger)logger));
            endpointConfiguration.SendOnly();
            return endpointConfiguration;
        }
    }
}