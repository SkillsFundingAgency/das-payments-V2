using Autofac;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.Submission;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Infrastructure.Ioc
{
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

            builder.RegisterType<RecordPeriodEndStopJobHandler>()
                .As<IHandleMessageBatches<RecordPeriodEndStopJob>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RecordPeriodEndRunJobHandler>()
                .As<IHandleMessageBatches<RecordPeriodEndRunJob>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RecordPeriodEndStartJobHandler>()
                .As<IHandleMessageBatches<RecordPeriodEndStartJob>>()
                .InstancePerLifetimeScope();
        }
    }
}