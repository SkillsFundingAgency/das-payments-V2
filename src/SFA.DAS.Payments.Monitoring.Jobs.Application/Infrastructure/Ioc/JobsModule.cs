using Autofac;
using Microsoft.Extensions.Caching.Memory;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Ioc
{
    public class JobsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new JobsDataContext(configHelper.GetConnectionString("PaymentsMonitoringConnectionString"));
                })
                .As<IJobsDataContext>()
                .InstancePerLifetimeScope();
            builder.RegisterType<EarningsJobService>()
                .As<IEarningsJobService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<JobStepService>()
                .As<IJobStepService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<MonthEndJobService>()
                .As<IMonthEndJobService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<JobStatusService>()
                .As<IJobStatusService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<CompletedJobsService>()
                .As<ICompletedJobsService>()
                .InstancePerLifetimeScope();
            builder.Register((c, p) => new MemoryCache(new MemoryCacheOptions()))
                .As<IMemoryCache>()
                .SingleInstance();
            builder.RegisterType<SqlExceptionService>()
                .As<ISqlExceptionService>()
                .SingleInstance();
            builder.RegisterBuildCallback(c =>
            {
                var config = c.Resolve<IApplicationConfiguration>();
                EndpointConfigurationEvents.ConfiguringTransport += (object sender, TransportExtensions<AzureServiceBusTransport> e) =>
                {
                    e.Routing().RouteToEndpoint(typeof(RecordStartedProcessingEarningsJob).Assembly, config.EndpointName);
                };
            });
        }
    }
}