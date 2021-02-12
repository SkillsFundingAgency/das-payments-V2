using System;
using Autofac;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.DateTimeProvider;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Queueing.Interface.Configuration;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.JobContextMessageHandling.JobStatus;
using SFA.DAS.Payments.Monitoring.Jobs.Data;

namespace SFA.DAS.Payments.JobContextMessageHandling.Infrastructure.Ioc
{
    public class JobContextManagementModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobContextManager<JobContextMessage>>()
                           .As<IJobContextManager<JobContextMessage>>()
                           .SingleInstance();

            builder.RegisterType<DefaultJobContextMessageMapper<JobContextMessage>>().As<IMapper<JobContextMessage, JobContextMessage>>();
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>();

            builder.Register(c =>
            {
                var configHelper = c.Resolve<IConfigurationHelper>();
                return new TopicConfiguration(
                    configHelper.GetConnectionString("DCServiceBusConnectionString"),
                    configHelper.GetSetting("TopicName"),
                    configHelper.GetSetting("SubscriptionName"),
                    1,
                    maximumCallbackTimeSpan: TimeSpan.Parse(configHelper.GetSettingOrDefault("MaximumCallbackTimeSpan", "02:00:00")));
            })
                .As<ITopicConfiguration>();

            builder.Register(c =>
            {
                var topicSubscriptionConfig = c.Resolve<ITopicConfiguration>();

                return new TopicSubscriptionSevice<JobContextDto>(
                    topicSubscriptionConfig,
                    c.Resolve<IJsonSerializationService>(),
                    c.Resolve<ILogger>());
            }).As<ITopicSubscriptionService<JobContextDto>>();

            builder.Register(c =>
            {
                var config = c.Resolve<ITopicConfiguration>();
                var serialisationService = c.Resolve<IJsonSerializationService>();
                return new TopicPublishService<JobContextDto>(config, serialisationService);
            }).As<ITopicPublishService<JobContextDto>>();

            builder.Register(c =>
            {
                var configHelper = c.Resolve<IConfigurationHelper>();
                var auditPublishConfig = new QueueConfiguration(configHelper.GetConnectionString("DCServiceBusConnectionString"), configHelper.GetSetting("AuditQueueName"), 1);

                return new QueuePublishService<AuditingDto>(
                    auditPublishConfig,
                    c.Resolve<IJsonSerializationService>());
            }).As<IQueuePublishService<AuditingDto>>();

            builder.Register(c =>
            {
                var configHelper = c.Resolve<IConfigurationHelper>();
                var jobStatusPublishConfig = new QueueConfiguration(configHelper.GetConnectionString("DCServiceBusConnectionString"), configHelper.GetSetting("JobStatusQueueName"), 1);

                return new QueuePublishService<JobStatusDto>(
                    jobStatusPublishConfig,
                    c.Resolve<IJsonSerializationService>());
            }).As<IQueuePublishService<JobStatusDto>>();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService, ISerializationService>();

            builder.RegisterType<JobContextManagerService>()
                .As<IJobContextManagerService>()
                .SingleInstance();

            builder.Register(c =>
                {
                    var config = c.Resolve<IConfigurationHelper>();
                    var periodEndConfig = new JobStatusConfiguration
                    {
                        TimeToPauseBetweenChecks = TimeSpan.Parse(config.GetSettingOrDefault("TimeToPauseBetweenChecks", "00:01:30")),
                        TimeToWaitForJobToComplete = TimeSpan.Parse(config.GetSettingOrDefault("TimeToWaitForJobToComplete", "00:30:30")),
                        TimeToWaitForPeriodEndRunJobToComplete = TimeSpan.Parse(config.GetSettingOrDefault("TimeToWaitForPeriodEndRunJobToComplete", "00:30:30"))
                    };
                    return periodEndConfig;
                })
                .As<IJobStatusConfiguration>()
                .SingleInstance();
            builder.RegisterType<JobStatusService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new JobsDataContext(configHelper.GetConnectionString("PaymentsConnectionString"));
                })
                .As<IJobsDataContext>()
                .InstancePerLifetimeScope();
        }
    }
}