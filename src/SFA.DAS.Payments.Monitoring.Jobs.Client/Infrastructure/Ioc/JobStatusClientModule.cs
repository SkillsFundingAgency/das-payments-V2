using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure.Ioc
{
    public class JobStatusClientModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProviderEarningsJobClient>()
                .As<IProviderEarningsJobClient>()
                .SingleInstance();

            builder.RegisterType<ProviderEarningsJobClientFactory>()
                .As<IProviderEarningsJobClientFactory>()
                .SingleInstance();

            builder.RegisterType<JobStatusIncomingMessageBehaviour>()
                .SingleInstance();

            builder.RegisterType<JobStatusOutgoingMessageBehaviour>()
                .SingleInstance();


            builder.RegisterBuildCallback(c =>
            {
                var config = c.Resolve<IConfigurationHelper>();
                var jobStatusEndpointName = config.GetSettingOrDefault("monitoring-jobs-endpoint", "sfa-das-payments-monitoring-jobs");
                EndpointConfigurationEvents.ConfiguringTransport += (object sender, TransportExtensions<AzureServiceBusTransport> e) =>
                {
                    e.Routing().RouteToEndpoint(typeof(RecordStartedProcessingProviderEarningsJob).Assembly, jobStatusEndpointName);
                };
                var endpointConfig = c.Resolve<EndpointConfiguration>();
                endpointConfig.Notifications.Errors.MessageSentToErrorQueue += Errors_MessageSentToErrorQueue;
            });

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
    }
}