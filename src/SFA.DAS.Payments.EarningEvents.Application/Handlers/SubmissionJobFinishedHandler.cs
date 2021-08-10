using System;
using System.Threading.Tasks;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Queueing.Interface;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Handlers
{
    public class SubmissionJobFinishedHandler : IHandleMessages<SubmissionJobFinishedEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IQueuePublishService<JobStatusDto> jobStatusDtoQueuePublishService;

        public SubmissionJobFinishedHandler(IPaymentLogger logger, IQueuePublishService<JobStatusDto> jobStatusDtoQueuePublishService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobStatusDtoQueuePublishService = jobStatusDtoQueuePublishService ?? throw new ArgumentNullException(nameof(this.jobStatusDtoQueuePublishService));
        }

        public async Task Handle(SubmissionJobFinishedEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Handling SubmissionJobFinished event for Ukprn: {message.Ukprn}");

            if (message is SubmissionJobSucceeded)
            {
                await jobStatusDtoQueuePublishService.PublishAsync(new JobStatusDto(message.JobId, (int)JobStatusType.Completed));
                return;
            }

            if (message is SubmissionJobFailed)
            {
                await jobStatusDtoQueuePublishService.PublishAsync(new JobStatusDto(message.JobId, (int)JobStatusType.Failed));
                return;
            }

            logger.LogInfo($"Finished handling SubmissionJobFinished event for Ukprn: {message.Ukprn}");
            throw new InvalidOperationException("Unable to resolve job status");

            
        }
    }
}