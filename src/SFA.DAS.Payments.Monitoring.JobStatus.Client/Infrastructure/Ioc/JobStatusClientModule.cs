using Autofac;
using NServiceBus;
using NServiceBus.MessageMutator;
using SFA.DAS.Payments.Monitoring.JobStatus.Client.Infrastructure.Messaging;

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

            builder.RegisterBuildCallback(c =>
                c.Resolve<EndpointConfiguration>().Pipeline.Register(typeof(JobStatusIncomingMessageBehaviour),"Job Status Incoming message behaviour"));

            builder.RegisterBuildCallback(c =>
                c.Resolve<EndpointConfiguration>().Pipeline.Register(typeof(JobStatusOutgoingMessageBehaviour), "Job Status Outgoing message behaviour"));
        }
    }
}