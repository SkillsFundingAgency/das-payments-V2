using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.Queueing.Interface;
using Newtonsoft.Json;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.JobContextMessageHandling
{
    public class DasJobContextManager<T> : IJobContextManager<T>, IHandleMessages<SubmissionJobFinishedEvent>
        where T : class
    {
        private readonly ITopicSubscriptionService<JobContextDto> topicSubscriptionService;
        private readonly ITopicPublishService<JobContextDto> topicPublishService;
        private readonly IMapper<JobContextMessage, T> mapper;
        private readonly IQueuePublishService<JobStatusDto> jobStatusDtoQueuePublishService;
        private readonly IQueuePublishService<AuditingDto> auditingDtoQueuePublishService;
        private readonly IJobsDataContext jobsDataContext;
        private readonly IPaymentLogger logger;
        private readonly IMessageHandler<T> messageHandler;
        private readonly IMapper<JobContextDto, JobContextMessage> jobContextDtoToMessageMapper;
        private readonly IJobContextMessageMetadataService jobContextMessageMetadataService;

        public DasJobContextManager(
            ITopicSubscriptionService<JobContextDto> topicSubscriptionService,
            ITopicPublishService<JobContextDto> topicPublishService,
            IMapper<JobContextMessage, T> mapper,
            IQueuePublishService<JobStatusDto> jobStatusDtoQueuePublishService,
            IQueuePublishService<AuditingDto> auditingDtoQueuePublishService,
            IJobsDataContext jobsDataContext,
            IPaymentLogger logger,
            IMessageHandler<T> messageHandler)
            : this(
                topicSubscriptionService,
                topicPublishService,
                mapper,
                jobStatusDtoQueuePublishService,
                auditingDtoQueuePublishService,
                jobsDataContext,
                logger,
                messageHandler,
                new JobContextDtoToMessageMapper(),
                new JobContextMessageMetadataService())
        {
        }

        public DasJobContextManager(
            ITopicSubscriptionService<JobContextDto> topicSubscriptionService,
            ITopicPublishService<JobContextDto> topicPublishService,
            IMapper<JobContextMessage, T> mapper,
            IQueuePublishService<JobStatusDto> jobStatusDtoQueuePublishService,
            IQueuePublishService<AuditingDto> auditingDtoQueuePublishService,
            IJobsDataContext jobsDataContext,
            IPaymentLogger logger,
            IMessageHandler<T> messageHandler,
            IMapper<JobContextDto, JobContextMessage> jobContextDtoToMessageMapper,
            IJobContextMessageMetadataService jobContextMessageMetadataService)
        {
            this.topicSubscriptionService = topicSubscriptionService;
            this.topicPublishService = topicPublishService;
            this.mapper = mapper;
            this.jobStatusDtoQueuePublishService = jobStatusDtoQueuePublishService;
            this.auditingDtoQueuePublishService = auditingDtoQueuePublishService;
            this.jobsDataContext = jobsDataContext;
            this.logger = logger;
            this.messageHandler = messageHandler;
            this.jobContextDtoToMessageMapper = jobContextDtoToMessageMapper;
            this.jobContextMessageMetadataService = jobContextMessageMetadataService;
        }

        public void OpenAsync(CancellationToken cancellationToken)
        {
            logger.LogInfo("Opening Job Context Manager method invoked, Topic Subscription Subscribing");
            topicSubscriptionService.Subscribe(Callback, cancellationToken);
        }

        public async Task CloseAsync()
        {
            logger.LogInfo("Closing Job Context Manager method invoked, Topic Subscription Unsubscribing");
            await topicSubscriptionService.UnsubscribeAsync();
        }

        private async Task<IQueueCallbackResult> Callback(JobContextDto jobContextDto, IDictionary<string, object> messageProperties, CancellationToken cancellationToken)
        {
            JobContextMessage jobContextMessage = jobContextDtoToMessageMapper.MapTo(jobContextDto);

            try
            {
                if (jobContextMessageMetadataService.PointerIsFirstTopic(jobContextMessage))
                {
                    await auditingDtoQueuePublishService.PublishAsync(jobContextMessageMetadataService.BuildAuditingDto(jobContextMessage, AuditEventType.JobStarted));

                    if (!jobContextMessageMetadataService.ShoudFailJob(jobContextMessage))
                    {
                        await jobStatusDtoQueuePublishService.PublishAsync(jobContextMessageMetadataService.BuildJobStatusDto(jobContextMessage, JobStatusType.Processing));
                    }
                }

                await auditingDtoQueuePublishService.PublishAsync(jobContextMessageMetadataService.BuildAuditingDto(jobContextMessage, AuditEventType.ServiceStarted));

                T obj = mapper.MapTo(jobContextMessage);

                var hasHandlerSucceeded = await messageHandler.HandleAsync(obj, cancellationToken);

                if (!hasHandlerSucceeded)
                {
                    await PublishFailMessage(jobContextMessage);
                    await auditingDtoQueuePublishService.PublishAsync(jobContextMessageMetadataService.BuildAuditingDto(jobContextMessage, AuditEventType.JobFailed));
                }

                return new QueueCallbackResult(true, null);
            }
            catch (Exception ex)
            {
                logger.LogError("Exception thrown in JobContextManager callback", ex, jobIdOverride: jobContextDto.JobId);

                await auditingDtoQueuePublishService.PublishAsync(jobContextMessageMetadataService.BuildAuditingDto(jobContextMessage, AuditEventType.JobFailed, ex.ToString()));

                var result = await PublishFailMessage(jobContextMessage);
                return new QueueCallbackResult(result, ex);
            }
        }

        private async Task<bool> PublishFailMessage(JobContextMessage jobContextMessage)
        {
            var shouldFailJob = jobContextMessageMetadataService.ShoudFailJob(jobContextMessage);
            if (shouldFailJob)
            {
                logger.LogInfo($"Will be sending message to fail the job for job id : {jobContextMessage.JobId}", null, jobContextMessage.JobId);
                await jobStatusDtoQueuePublishService.PublishAsync(jobContextMessageMetadataService.BuildJobStatusDto(jobContextMessage, JobStatusType.Failed));
            }

            return shouldFailJob;
        }

        private async Task ContinueProcessingJobContextMessage(bool dasJobSucceeded, long dcJobId)
        {
            var job = await jobsDataContext.GetJobByDcJobId(dcJobId);

            var jobContextMessage = JsonConvert.DeserializeObject<JobContextMessage>(job.JobContextMessagePayload);

            await auditingDtoQueuePublishService.PublishAsync(jobContextMessageMetadataService.BuildAuditingDto(jobContextMessage, AuditEventType.ServiceFinished));

            if (jobContextMessageMetadataService.PointerIsLastTopic(jobContextMessage))
            {
                await auditingDtoQueuePublishService.PublishAsync(jobContextMessageMetadataService.BuildAuditingDto(jobContextMessage, AuditEventType.JobFinished));

                if (dasJobSucceeded)
                {
                    await jobStatusDtoQueuePublishService.PublishAsync(jobContextMessageMetadataService.BuildJobStatusDto(jobContextMessage, JobStatusType.Completed));
                }
                else
                {
                    await jobStatusDtoQueuePublishService.PublishAsync(jobContextMessageMetadataService.BuildJobStatusDto(jobContextMessage, JobStatusType.Failed));
                }
            }
            else
            {
                jobContextMessage.TopicPointer++;

                var nextTopicSubscriptionName = jobContextMessage.Topics[jobContextMessage.TopicPointer].SubscriptionName;

                var nextTopicProperties = new Dictionary<string, object>
                {
                    { "To", nextTopicSubscriptionName }
                };

                var nextTopicJobContextDto = jobContextDtoToMessageMapper.MapFrom(jobContextMessage);
                await topicPublishService.PublishAsync(nextTopicJobContextDto, nextTopicProperties, nextTopicSubscriptionName);
            }
        }

        public async Task Handle(SubmissionJobFinishedEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Handling SubmissionJobFinished event for Ukprn: {message.Ukprn}");
            bool dasJobStatus;

            switch (message)
            {
                case SubmissionJobSucceeded _:
                    dasJobStatus = true;
                    break;
                case SubmissionJobFailed _:
                    dasJobStatus = false;
                    break;
                default:
                    throw new InvalidOperationException("Unable to resolve job status");
            }

            await ContinueProcessingJobContextMessage(dasJobStatus, message.JobId);

            logger.LogInfo($"Finished handling SubmissionJobFinished event for Ukprn: {message.Ukprn}");

        }
    }
}