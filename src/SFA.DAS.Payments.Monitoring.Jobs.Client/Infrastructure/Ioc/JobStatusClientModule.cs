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
            builder.RegisterType<EarningsJobClient>()
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
                var config = c.Resolve<IConfigurationHelper>();
                var jobsEndpointName = config.GetSettingOrDefault("Monitoring_JobsService_EndpointName", "sfa-das-payments-monitoring-jobs");
                EndpointConfigurationEvents.ConfiguringTransport += (object sender, TransportExtensions<AzureServiceBusTransport> e) =>
                {
                    e.Routing().RouteToEndpoint(typeof(RecordStartedProcessingEarningsJob).Assembly, jobsEndpointName);
                };
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