using Autofac;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using NServiceBus;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Infrastructure.Ioc
{
    public class JobServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobStorageService>()
                .As<IJobStorageService>()
                .InstancePerLifetimeScope();

            EndpointConfigurationEvents.EndpointConfigured += EndpointConfigurationEvents_EndpointConfigured;
        }

        private void EndpointConfigurationEvents_EndpointConfigured(object sender, EndpointConfiguration e)
        {
            e.LimitMessageProcessingConcurrencyTo(20);
        }
    }
}