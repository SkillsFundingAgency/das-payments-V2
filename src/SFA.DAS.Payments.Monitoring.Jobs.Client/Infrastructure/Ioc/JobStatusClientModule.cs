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
            builder.RegisterType<ProviderEarningsJobStatusClient>()
                .As<IProviderEarningsJobStatusClient>()
                .SingleInstance();

            builder.RegisterType<ProviderEarningsJobStatusClientFactory>()
                .As<IProviderEarningsJobStatusClientFactory>()
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
            });

            builder.RegisterBuildCallback(c =>
                c.Resolve<EndpointConfiguration>().Pipeline.Register(typeof(JobStatusIncomingMessageBehaviour),"Job Status Incoming message behaviour"));

            builder.RegisterBuildCallback(c =>
                c.Resolve<EndpointConfiguration>().Pipeline.Register(typeof(JobStatusOutgoingMessageBehaviour), "Job Status Outgoing message behaviour"));
        }

    }
}