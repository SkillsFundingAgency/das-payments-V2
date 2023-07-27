using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.JobContextMessageHandling.Infrastructure;
using SFA.DAS.Payments.JobContextMessageHandling.JobStatus;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.PeriodEnd.Model;

namespace SFA.DAS.Payments.PeriodEnd.Application.Handlers
{
    public class PeriodEndJobContextMessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly IEndpointInstanceFactory endpointInstanceFactory;
        private readonly IPeriodEndJobClient jobClient;
        private readonly IJobsDataContext jobsDataContext;
        private readonly IJobStatusService jobStatusService;
        private readonly IPaymentLogger logger;

        public PeriodEndJobContextMessageHandler(IPaymentLogger logger,
            IEndpointInstanceFactory endpointInstanceFactory, IPeriodEndJobClient jobClient,
            IJobStatusService jobStatusService, IJobsDataContext jobsDataContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.endpointInstanceFactory = endpointInstanceFactory ??
                                           throw new ArgumentNullException(nameof(endpointInstanceFactory));
            this.jobClient = jobClient ?? throw new ArgumentNullException(nameof(jobClient));
            this.jobStatusService = jobStatusService ?? throw new ArgumentNullException(nameof(jobStatusService));
            this.jobsDataContext = jobsDataContext ?? throw new ArgumentNullException(nameof(jobsDataContext));
        }

        public async Task<bool> HandleAsync(JobContextMessage message, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogDebug("Getting task type from period end message.");
                var taskType = GetTask(message);
                logger.LogDebug("Got period end type now create the period end event.");
                var periodEndEvent = CreatePeriodEndEvent(taskType);
                logger.LogDebug($"Created period end event. Type: {periodEndEvent.GetType().Name}");
                periodEndEvent.JobId = message.JobId;
                periodEndEvent.CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = Convert.ToInt16(GetMessageValue(message,
                        JobContextMessageConstants.KeyValuePairs.CollectionYear)),
                    Period = Convert.ToByte(GetMessageValue(message,
                        JobContextMessageConstants.KeyValuePairs.ReturnPeriod))
                };

                logger.LogDebug($"Got period end event: {periodEndEvent.ToJson()}");

                var jobIdToWaitFor = message.JobId;

                if (taskType == PeriodEndTaskType.PeriodEndReports ||
                    taskType == PeriodEndTaskType.PeriodEndSubmissionWindowValidation)
                {
                    await RecordPeriodEndJob(taskType, periodEndEvent).ConfigureAwait(false);
                    var endpointInstance = await endpointInstanceFactory.GetEndpointInstance();
                    await endpointInstance.Publish(periodEndEvent);
                    logger.LogInfo(
                        $"Finished publishing the period end event. Name: {periodEndEvent.GetType().Name}, JobId: {periodEndEvent.JobId}, Collection Period: {periodEndEvent.CollectionPeriod.Period}-{periodEndEvent.CollectionPeriod.AcademicYear}.");
                }
                else
                {
                    var existingNonFailedJobId = await jobsDataContext.GetNonFailedDcJobId(GetJobType(taskType),
                        periodEndEvent.CollectionPeriod.AcademicYear, periodEndEvent.CollectionPeriod.Period);

                    if (existingNonFailedJobId.GetValueOrDefault() == 0)
                    {
                        await RecordPeriodEndJob(taskType, periodEndEvent).ConfigureAwait(false);

                        var endpointInstance = await endpointInstanceFactory.GetEndpointInstance();
                        await endpointInstance.Publish(periodEndEvent);
                        logger.LogInfo(
                            $"Finished publishing the period end event. Name: {periodEndEvent.GetType().Name}, JobId: {periodEndEvent.JobId}, Collection Period: {periodEndEvent.CollectionPeriod.Period}-{periodEndEvent.CollectionPeriod.AcademicYear}.");
                    }
                    else
                    {
                        jobIdToWaitFor = existingNonFailedJobId.GetValueOrDefault();
                        logger.LogWarning(
                            $"Job already exists, will not be published. Name: {periodEndEvent.GetType().Name}, JobId: {periodEndEvent.JobId}, Collection Period: {periodEndEvent.CollectionPeriod.Period}-{periodEndEvent.CollectionPeriod.AcademicYear}.");
                    }
                }

                // TODO: This is a temporary workaround to enable the PeriodEndStart and PeriodEndStop messages to return true as otherwise the service will
                // TODO: just hang as there is nothing implemented to handle the Start and Stop events and so the job status service will never get a completion and so this will never return true.
                // PV2-1345 will handle PeriodEndStart
                // PeriodEndStoppedEvent will be handled by the PeriodEndStoppedEventHandler which in turn is handled by the ProcessProviderMonthEndCommandHandler but we don't want to wait for it

                if (periodEndEvent is PeriodEndStoppedEvent)
                {
                    logger.LogDebug("Returning as this is a PeriodEndStop event");
                    return true;
                }

                if (periodEndEvent is PeriodEndStartedEvent ||
                    periodEndEvent is PeriodEndRequestValidateSubmissionWindowEvent ||
                    periodEndEvent is PeriodEndRequestReportsEvent ||
                    periodEndEvent is PeriodEndIlrReprocessingStartedEvent)
                    return await jobStatusService.WaitForPeriodEndJobToFinish(jobIdToWaitFor, cancellationToken);

                if (periodEndEvent is PeriodEndRunningEvent)
                    await jobStatusService.WaitForPeriodEndRunJobToFinish(jobIdToWaitFor, cancellationToken);
                else
                    await jobStatusService.WaitForJobToFinish(jobIdToWaitFor, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to process job context message. Message: {message.ToJson()}", ex);
                throw;
            }
        }

        private async Task RecordPeriodEndJob(PeriodEndTaskType taskType, PeriodEndEvent periodEndEvent)
        {
            logger.LogDebug($"Recording period end job. Type: {taskType:G}");
            var generatedMessage = new GeneratedMessage
            {
                MessageId = periodEndEvent.EventId,
                MessageName = periodEndEvent.GetType().FullName,
                StartTime = periodEndEvent.EventTime
            };

            if (taskType == PeriodEndTaskType.PeriodEndStart ||
                taskType == PeriodEndTaskType.PeriodEndRun ||
                taskType == PeriodEndTaskType.PeriodEndStop ||
                taskType == PeriodEndTaskType.PeriodEndSubmissionWindowValidation ||
                taskType == PeriodEndTaskType.PeriodEndReports ||
                taskType == PeriodEndTaskType.PeriodEndIlrReprocessing)
            {
                var job = CreatePeriodEndJob(taskType);

                job.JobId = periodEndEvent.JobId;
                job.CollectionYear = periodEndEvent.CollectionPeriod.AcademicYear;
                job.CollectionPeriod = periodEndEvent.CollectionPeriod.Period;
                job.GeneratedMessages = new List<GeneratedMessage> { generatedMessage };

                await jobClient.StartPeriodEndJob(job).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException($"Unhandled period end task type: {taskType:G}");
            }
        }

        private PeriodEndTaskType GetTask(JobContextMessage message)
        {
            var taskName = message.Topics?.FirstOrDefault()?.Tasks.FirstOrDefault()?.Tasks.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(taskName))
                throw new InvalidOperationException(
                    "Invalid period end message, cannot determine the type of message from the topics & tasks.");

            if (!Enum.TryParse<PeriodEndTaskType>(taskName, out var taskType))
                throw new InvalidOperationException($"Invalid period end task type: '{taskName}'");

            logger.LogDebug($"Got task type: {taskType:G}");
            return taskType;
        }

        private PeriodEndEvent CreatePeriodEndEvent(PeriodEndTaskType taskType)
        {
            switch (taskType)
            {
                case PeriodEndTaskType.PeriodEndStart:
                    return new PeriodEndStartedEvent();

                case PeriodEndTaskType.PeriodEndRun:
                    return new PeriodEndRunningEvent();

                case PeriodEndTaskType.PeriodEndStop:
                    return new PeriodEndStoppedEvent();

                case PeriodEndTaskType.PeriodEndSubmissionWindowValidation:
                    return new PeriodEndRequestValidateSubmissionWindowEvent();

                case PeriodEndTaskType.PeriodEndReports:
                    return new PeriodEndRequestReportsEvent();

                case PeriodEndTaskType.PeriodEndIlrReprocessing:
                    return new PeriodEndIlrReprocessingStartedEvent();

                default:
                    throw new InvalidOperationException($"Cannot handle period end task type: '{taskType:G}'");
            }
        }

        private RecordPeriodEndJob CreatePeriodEndJob(PeriodEndTaskType periodEndTaskType)
        {
            switch (periodEndTaskType)
            {
                case PeriodEndTaskType.PeriodEndStart:
                    return new RecordPeriodEndStartJob();

                case PeriodEndTaskType.PeriodEndRun:
                    return new RecordPeriodEndRunJob();

                case PeriodEndTaskType.PeriodEndStop:
                    return new RecordPeriodEndStopJob();

                case PeriodEndTaskType.PeriodEndSubmissionWindowValidation:
                    return new RecordPeriodEndSubmissionWindowValidationJob();

                case PeriodEndTaskType.PeriodEndReports:
                    return new RecordPeriodEndRequestReportsJob();

                case PeriodEndTaskType.PeriodEndIlrReprocessing:
                    return new RecordPeriodEndIlrReprocessingStartedJob();

                default:
                    throw new InvalidOperationException($"Unhandled period end task type: {periodEndTaskType:G}");
            }
        }

        private JobType GetJobType(PeriodEndTaskType periodEndTaskType)
        {
            switch (periodEndTaskType)
            {
                case PeriodEndTaskType.PeriodEndStart:
                    return JobType.PeriodEndStartJob;

                case PeriodEndTaskType.PeriodEndRun:
                    return JobType.PeriodEndRunJob;

                case PeriodEndTaskType.PeriodEndStop:
                    return JobType.PeriodEndStopJob;

                case PeriodEndTaskType.PeriodEndIlrReprocessing:
                    return JobType.PeriodEndIlrReprocessingJob;

                default:
                    throw new InvalidOperationException($"Cannot handle period end task type: '{periodEndTaskType:G}'");
            }
        }

        private object GetMessageValue(JobContextMessage message, string key)
        {
            return message.KeyValuePairs?[key] ??
                   throw new InvalidOperationException($"No valid value found for key:{key}");
        }
    }
}