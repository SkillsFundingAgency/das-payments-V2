using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Caching.Memory;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobMessageProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.Earnings;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Ioc
{
    public class JobsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new JobsDataContext(configHelper.GetConnectionString("PaymentsConnectionString"));
                })
                .As<IJobsDataContext>()
                .InstancePerLifetimeScope();
            builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<IConfigurationHelper>();
                return new JobServiceConfiguration(
                    TimeSpan.Parse(configHelper.GetSettingOrDefault("JobStatusCheck_Interval", "00:00:10")),
                    TimeSpan.Parse(configHelper.GetSettingOrDefault("TimeToWaitForJobToComplete", "00:20:00"))
                    );
            })
                .As<IJobServiceConfiguration>()
                .SingleInstance();
            builder.RegisterType<EarningsJobStatusManager>()
                .As<IEarningsJobStatusManager>()
                .SingleInstance();
            builder.RegisterType<PeriodEndJobStatusManager>()
                .As<IPeriodEndJobStatusManager>()
                .SingleInstance();
            builder.RegisterType<PeriodEndStartJobStatusManager>()
                .As<IPeriodEndStartJobStatusManager>()
                .SingleInstance();
            builder.RegisterType<JobService>()
                .As<ICommonJobService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<EarningsJobService>()
                .As<IEarningsJobService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndJobService>()
                .As<IPeriodEndJobService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<JobMessageService>()
                .As<IJobMessageService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<EarningsJobStatusService>()
                .As<IEarningsJobStatusService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndJobStatusService>()
                .As<IPeriodEndJobStatusService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndStartJobStatusService>()
                .As<IPeriodEndStartJobStatusService>()
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
                    e.Routing().RouteToEndpoint(typeof(RecordEarningsJob).Assembly, config.EndpointName);
                };
            });

            builder.RegisterType<JobStatusEventPublisher>()
                .As<IJobStatusEventPublisher>()
                .InstancePerLifetimeScope();

            //TODO: should not be in here
            builder.RegisterType<ActorReliableCollectionCache<JobModel>>()
                .As<IActorDataCache<JobModel>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ActorReliableCollectionCache<JobStepModel>>()
                .As<IActorDataCache<JobStepModel>>()
                //.AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterType<ActorReliableCollectionCache<List<Guid>>>()
                .As<IActorDataCache<List<Guid>>>()
                //.AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterType<ActorReliableCollectionCache<(JobStepStatus jobStatus, DateTimeOffset? endTime)>>()
                .As<IActorDataCache<(JobStepStatus jobStatus, DateTimeOffset? endTime)>>()
                //.AsImplementedInterfaces()
                .InstancePerLifetimeScope();


        }
    }
}