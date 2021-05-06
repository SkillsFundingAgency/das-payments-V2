using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.PeriodEnd.Model;
using NServiceBus;
using SFA.DAS.Payments.JobContextMessageHandling.Infrastructure;
using SFA.DAS.Payments.JobContextMessageHandling.JobStatus;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.PeriodEnd.Application.Handlers
{
    public class PeriodEndJobContextMessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly IPaymentLogger logger;
        private readonly IEndpointInstanceFactory endpointInstanceFactory;
        private readonly IPeriodEndJobClient jobClient;
        private readonly IJobStatusService jobStatusService;
        private readonly IJobsDataContext jobsDataContext;

        public PeriodEndJobContextMessageHandler(IPaymentLogger logger,
            IEndpointInstanceFactory endpointInstanceFactory, IPeriodEndJobClient jobClient, IJobStatusService jobStatusService, IJobsDataContext jobsDataContext)
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
                    AcademicYear = Convert.ToInt16(GetMessageValue(message, JobContextMessageConstants.KeyValuePairs.CollectionYear)),
                    Period = Convert.ToByte(GetMessageValue(message, JobContextMessageConstants.KeyValuePairs.ReturnPeriod))
                };

                logger.LogDebug($"Got period end event: {periodEndEvent.ToJson()}");

                var jobIdToWaitFor = message.JobId;

                //todo combine these two if statements, same logic
                if (taskType == PeriodEndTaskType.PeriodEndReports)
                {
                    await RecordPeriodEndJob(taskType, periodEndEvent).ConfigureAwait(false);
                    var endpointInstance = await endpointInstanceFactory.GetEndpointInstance();
                    await endpointInstance.Publish(periodEndEvent);
                    logger.LogInfo(
                        $"Finished publishing the period end event. Name: {periodEndEvent.GetType().Name}, JobId: {periodEndEvent.JobId}, Collection Period: {periodEndEvent.CollectionPeriod.Period}-{periodEndEvent.CollectionPeriod.AcademicYear}.");
                }
                else if(taskType == PeriodEndTaskType.PeriodEndSubmissionWindowValidation)
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
                        logger.LogWarning($"Job already exists, will not be published. Name: {periodEndEvent.GetType().Name}, JobId: {periodEndEvent.JobId}, Collection Period: {periodEndEvent.CollectionPeriod.Period}-{periodEndEvent.CollectionPeriod.AcademicYear}.");
                    }
                }

                // TODO: This is a temporary workaround to enable the PeriodEndStart and PeriodEndStop messages to return true as otherwise the service will
                // TODO: just hang as there is nothing implemented to handle the Start and Stop events and so the job status service will never get a completion and so this will never return true.
                // PV2-1345 will handle PeriodEndStart
                // PeriodEndStoppedEvent will be handled by the PeriodEndStoppedEventHandler which in turn is handled by the ProcessProviderMonthEndCommandHandler but we don't want to wait for it

                if (periodEndEvent is PeriodEndStoppedEvent)
                {
                    logger.LogDebug("Returning as this is either a PeriodEndStop or PeriodEndRequestReports event");
                    return true;
                }

                //todo combine these three events to call new method on jobStatusService
                if (periodEndEvent is PeriodEndStartedEvent)
                {
                    return await jobStatusService.WaitForPeriodEndStartedToFinish(jobIdToWaitFor, cancellationToken);
                }

                if (periodEndEvent is PeriodEndRequestValidateSubmissionWindowEvent)
                {
                    return await jobStatusService.WaitForPeriodEndSubmissionWindowValidationToFinish(jobIdToWaitFor, cancellationToken);
                }

                if (periodEndEvent is PeriodEndRequestReportsEvent)
                {
                    return await jobStatusService.WaitForPeriodEndRequestReportsJobToFinish(jobIdToWaitFor, cancellationToken);
                }

                if (periodEndEvent is PeriodEndRunningEvent)
                {
                    await jobStatusService.WaitForPeriodEndRunJobToFinish(jobIdToWaitFor, cancellationToken);
                }
                else
                {
                    await jobStatusService.WaitForJobToFinish(jobIdToWaitFor, cancellationToken);
                }

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
            switch (taskType)
            {
                case PeriodEndTaskType.PeriodEndStart:
                    await jobClient.RecordPeriodEndStart(periodEndEvent.JobId, periodEndEvent.CollectionPeriod.AcademicYear,
                        periodEndEvent.CollectionPeriod.Period, new List<GeneratedMessage> { generatedMessage }).ConfigureAwait(false);
                    break;
                case PeriodEndTaskType.PeriodEndRun:
                    await jobClient.RecordPeriodEndRun(periodEndEvent.JobId, periodEndEvent.CollectionPeriod.AcademicYear,
                        periodEndEvent.CollectionPeriod.Period, new List<GeneratedMessage> { generatedMessage }).ConfigureAwait(false);
                    break;
                case PeriodEndTaskType.PeriodEndStop:
                    await jobClient.RecordPeriodEndStop(periodEndEvent.JobId, periodEndEvent.CollectionPeriod.AcademicYear,
                        periodEndEvent.CollectionPeriod.Period, new List<GeneratedMessage> { generatedMessage }).ConfigureAwait(false);
                    break;
                case PeriodEndTaskType.PeriodEndSubmissionWindowValidation:
                    await jobClient.RecordPeriodEndSubmissionWindowValidation(periodEndEvent.JobId, periodEndEvent.CollectionPeriod.AcademicYear,
                        periodEndEvent.CollectionPeriod.Period, new List<GeneratedMessage> { generatedMessage }).ConfigureAwait(false);
                    break;
                case PeriodEndTaskType.PeriodEndReports:
                    await jobClient.RecordPeriodEndRequestReportsJob(periodEndEvent.JobId, periodEndEvent.CollectionPeriod.AcademicYear,
                        periodEndEvent.CollectionPeriod.Period, new List<GeneratedMessage> { generatedMessage }).ConfigureAwait(false);
                    break;
                default:
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
                    return  new PeriodEndRequestReportsEvent();
                default:
                    throw new InvalidOperationException($"Cannot handle period end task type: '{taskType:G}'");
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