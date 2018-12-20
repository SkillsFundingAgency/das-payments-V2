using System.Collections.Generic;
using System.Linq;
using Autofac;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.MessageMutator;
using NServiceBus.Routing;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.JobStatus.Client.Infrastructure.Messaging;
using SFA.DAS.Payments.Monitoring.JobStatus.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Client.Infrastructure.Ioc
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

            builder.RegisterType<JobStatusContext>()
                .InstancePerLifetimeScope();

            builder.RegisterType<JobStatusIncomingMessageBehaviour>()
                .SingleInstance();

            builder.RegisterType<JobStatusOutgoingMessageBehaviour>()
                .SingleInstance();

            //builder.RegisterCallback(c => c.Registered += (object sender, Autofac.Core.ComponentRegisteredEventArgs e) =>
            //    {
            //        e.ComponentRegistration.Activated +=
            //            (object O, Autofac.Core.ActivatedEventArgs<object> activatedEventArgs) =>
            //            {
            //                if (activatedEventArgs.Instance is EndpointConfiguration)
            //                {
                                
            //                }
            //            };
            //    });

            builder.RegisterBuildCallback(c =>
            {
                var config = c.Resolve<IConfigurationHelper>();
                var jobStatusEndpointName = config.GetSettingOrDefault("monitoring-jobstatus-endpoint", "sfa-das-payments-monitoring-jobstatus");
                EndpointConfigurationEvents.ConfiguringTransport += (object sender, TransportExtensions<AzureServiceBusTransport> e) =>
                {
                    e.Routing().RouteToEndpoint(typeof(RecordStartedProcessingProviderEarningsJob).Assembly, jobStatusEndpointName);
                };
                var endpointConfig = c.Resolve<EndpointConfiguration>();
                //var settings = endpointConfig.GetSettings();
                //var routingTable =  settings.Get<UnicastRoutingTable>();
                //routingTable.AddOrReplaceRoutes("Monitoring-JobStatus",
                //    new List<RouteTableEntry>
                //    {
                //        new RouteTableEntry(typeof(RecordStartedProcessingProviderEarningsJob),
                //            UnicastRoute.CreateFromEndpointName(jobStatusEndpointName)),
                //        new RouteTableEntry(typeof(RecordJobMessageProcessingStatus),
                //            UnicastRoute.CreateFromEndpointName(jobStatusEndpointName))

                //    });
                //var transportConfig = c.Resolve<TransportExtensions<AzureServiceBusTransport>>();
                //var routing = transportConfig.Routing();
                //routing.RouteToEndpoint(typeof(RecordStartedProcessingProviderEarningsJob), config.GetSettingOrDefault("monitoring-jobstatus-endpoint", "sfa-das-payments-monitoring-jobstatus") );
                //routing.RouteToEndpoint(typeof(RecordJobMessageProcessingStatus), config.GetSettingOrDefault("monitoring-jobstatus-endpoint", "sfa-das-payments-monitoring-jobstatus"));
            });

            builder.RegisterBuildCallback(c =>
                c.Resolve<EndpointConfiguration>().Pipeline.Register(typeof(JobStatusIncomingMessageBehaviour),"Job Status Incoming message behaviour"));

            builder.RegisterBuildCallback(c =>
                c.Resolve<EndpointConfiguration>().Pipeline.Register(typeof(JobStatusOutgoingMessageBehaviour), "Job Status Outgoing message behaviour"));
        }

    }
}