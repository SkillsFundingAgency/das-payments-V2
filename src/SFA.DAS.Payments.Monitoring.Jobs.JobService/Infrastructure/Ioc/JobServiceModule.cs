using Autofac;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Infrastructure.Ioc
{
    public class JobServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobStorageService>()
                .As<IJobStorageService>()
                .InstancePerLifetimeScope();
        }
    }

    public class BatchMessageHandlersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RecordJobMessageProcessingStatusBatchHandler>()
                .As<IHandleMessageBatches<RecordJobMessageProcessingStatus>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RecordEarningsJobBatchHandler>()
                .As<IHandleMessageBatches<RecordEarningsJob>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RecordEarningsJobAdditionalMessageBatchHandler>()
                .As<IHandleMessageBatches<RecordEarningsJobAdditionalMessages>>()
                .InstancePerLifetimeScope();
        }
    }
}