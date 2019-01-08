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
    public interface IEarningsJobService
    {
        Task JobStarted(RecordStartedProcessingProviderEarningsJob startedEvent);
        Task JobStepCompleted(RecordJobMessageProcessingStatus stepCompleted);
    }

    public class EarningsJobService : IEarningsJobService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobsDataContext dataContext;
        private readonly ITelemetry telemetry;

        public EarningsJobService(IPaymentLogger logger, IJobsDataContext dataContext, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
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
            telemetry.AddProperty("JobType", JobType.EarningsJob.ToString("G"));
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

            if (completedStep.StartTime.HasValue)
            {
                telemetry.AddProperty("MessageName", completedStep.MessageName);
                telemetry.TrackDuration("Processed Message", completedStep.EndTime.Value - completedStep.StartTime.Value);
            }
            logger.LogInfo($"Recorded completion of message processing.  Job Id: {jobMessageStatus.JobId}, Message id: {jobMessageStatus.Id}.");
        }
    }
}