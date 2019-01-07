using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IProviderEarningsJobService
    {
        Task JobStarted(RecordStartedProcessingProviderEarningsJob startedEvent);
        Task JobStepCompleted(RecordJobMessageProcessingStatus stepCompleted);
    }

    public class ProviderEarningsJobService : IProviderEarningsJobService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobsDataContext dataContext;
        private readonly IJobsStatusServiceFacade jobsStatusService;
        private readonly ITelemetry telemetry;

        public ProviderEarningsJobService(IPaymentLogger logger, IJobsDataContext dataContext, IJobsStatusServiceFacade jobsStatusService, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.jobsStatusService = jobsStatusService ?? throw new ArgumentNullException(nameof(jobsStatusService));
            this.telemetry = telemetry;
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task JobStarted(RecordStartedProcessingProviderEarningsJob startedEvent)
        {
            logger.LogDebug($"Now recording new provider earnings job.  Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");
            var jobDetails = new JobModel
            {
                JobType = JobType.EarningsJob,
                StartTime = startedEvent.StartTime,
                CollectionPeriod = startedEvent.CollectionPeriod,
                CollectionYear = startedEvent.CollectionYear,
                Ukprn = startedEvent.Ukprn,
                DcJobId = startedEvent.JobId,
                IlrSubmissionTime = startedEvent.IlrSubmissionTime,
                Status = Data.Model.JobStatus.InProgress
            };
            var jobSteps = startedEvent.GeneratedMessages.Select(msg => new JobStepModel
            {
                MessageId = msg.MessageId,
                StartTime = msg.StartTime,
                MessageName = msg.MessageName,
                Status = JobStepStatus.Queued,

            }).ToList();
            await dataContext.SaveNewJob(jobDetails, jobSteps);
            telemetry.AddProperty("Ukprn", startedEvent.Ukprn.ToString());
            telemetry.AddProperty("JobId", startedEvent.JobId.ToString());
            telemetry.AddProperty("CollectionPeriod", startedEvent.CollectionPeriod.ToString());
            telemetry.AddProperty("CollectionYear", startedEvent.CollectionYear.ToString());
            telemetry.TrackEvent("Started Job");
            logger.LogInfo($"Finished saving the job to the db.  Job id: {jobDetails.Id}, DC Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");
        }

        public async Task JobStepCompleted(RecordJobMessageProcessingStatus jobMessageStatus)
        {
            logger.LogVerbose($"Now recording completion of message processing.  Job Id: {jobMessageStatus.JobId}, Message id: {jobMessageStatus.Id}.");
            var messageIds = new List<Guid> { jobMessageStatus.Id };
            messageIds.AddRange(jobMessageStatus.GeneratedMessages.Select(msg => msg.MessageId));
            var jobSteps = await dataContext.GetJobSteps(messageIds);
            var job = await dataContext.GetJobByDcJobId(jobMessageStatus.JobId) ?? throw new InvalidOperationException($"Job not found. Dc Job id: {jobMessageStatus.JobId}");
            var jobId = job.Id;
            var completedStep = jobSteps.FirstOrDefault(step => step.MessageId == jobMessageStatus.Id);
            if (completedStep == null)
            {
                logger.LogDebug($"Recording completion of job step before the start time of the step has been recorded. Job: {jobId}, Message Id: {jobMessageStatus.Id}");
                completedStep = new JobStepModel
                {
                    JobId = jobId,
                    MessageId = jobMessageStatus.Id,
                    MessageName = jobMessageStatus.MessageName,
                };
                jobSteps.Add(completedStep);
            }
            completedStep.Status = jobMessageStatus.Succeeded ? JobStepStatus.Completed : JobStepStatus.Failed;
            completedStep.EndTime = jobMessageStatus.EndTime;
            foreach (var generatedMessage in jobMessageStatus.GeneratedMessages)
            {
                var jobStep = jobSteps.FirstOrDefault(step => step.MessageId == generatedMessage.MessageId);
                if (jobStep == null)
                {
                    jobStep = new JobStepModel
                    {
                        JobId = jobId,
                        MessageId = generatedMessage.MessageId,
                        MessageName = generatedMessage.MessageName,
                        Status = JobStepStatus.Queued
                    };
                    jobSteps.Add(jobStep);
                }
                else
                    logger.LogDebug($"Updating job step to record the start time. Job id: {jobId}, Message id: {generatedMessage.MessageId}");
                jobStep.StartTime = generatedMessage.StartTime;
                jobStep.ParentMessageId = jobMessageStatus.Id;
            }

            await dataContext.SaveJobSteps(jobSteps);

            if (!jobMessageStatus.GeneratedMessages.Any())
            {
                logger.LogDebug($"No messages were generated as a result of processing this message therefore the job may have finished. Job: {jobId}.");
                var result = await jobsStatusService.JobStepsCompleted(jobId);
                if (result.Finished)
                {
                    telemetry.AddProperty("Ukprn", job.Ukprn?.ToString() ?? string.Empty);
                    telemetry.AddProperty("JobId", job.DcJobId?.ToString() ?? string.Empty);
                    telemetry.AddProperty("CollectionPeriod", job.CollectionPeriod.ToString());
                    telemetry.AddProperty("CollectionYear", job.CollectionYear.ToString());
                    telemetry.TrackDuration("Finished Job", result.endTime.Value - job.StartTime);
                }
            }
            if (completedStep.StartTime.HasValue)
            {
                telemetry.AddProperty("MessageName", completedStep.MessageName);
                telemetry.TrackDuration("Processed Message", completedStep.EndTime.Value - completedStep.StartTime.Value);
            }
            logger.LogInfo($"Recorded completion of message processing.  Job Id: {jobMessageStatus.JobId}, Message id: {jobMessageStatus.Id}.");
        }
    }
}