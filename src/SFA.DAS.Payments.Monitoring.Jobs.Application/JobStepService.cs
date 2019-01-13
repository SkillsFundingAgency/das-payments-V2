using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobStepService
    {
        Task JobStepCompleted(RecordJobMessageProcessingStatus jobMessageStatus);
    }

    public class JobStepService : IJobStepService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobsDataContext dataContext;
        private readonly ITelemetry telemetry;
        private readonly IMemoryCache cache;

        public JobStepService(IPaymentLogger logger, IJobsDataContext dataContext, ITelemetry telemetry, IMemoryCache cache)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task JobStepCompleted(RecordJobMessageProcessingStatus jobMessageStatus)
        {
            logger.LogVerbose($"Now recording completion of message processing.  Job Id: {jobMessageStatus.JobId}, Message id: {jobMessageStatus.Id}.");
            var messageIds = new List<Guid> { jobMessageStatus.Id };
            messageIds.AddRange(jobMessageStatus.GeneratedMessages.Select(msg => msg.MessageId));
            var jobSteps = await dataContext.GetJobSteps(messageIds);
            var job = await GetJob(jobMessageStatus.JobId);
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
                {
                    logger.LogDebug($"Updating job step to record the start time. Job id: {jobId}, Message id: {generatedMessage.MessageId}");
                }

                jobStep.StartTime = generatedMessage.StartTime;
                jobStep.ParentMessageId = jobMessageStatus.Id;
            }
            logger.LogVerbose("Now saving updated job steps.");
            await dataContext.SaveJobSteps(jobSteps);
            logger.LogDebug("Finished saving updated job steps to db.");
            SendTelemetry(job, jobSteps);
            logger.LogInfo($"Recorded completion of message processing.  Job Id: {jobMessageStatus.JobId}, Message id: {jobMessageStatus.Id}.");
        }

        private async Task<JobModel> GetJob(long dcJobId)
        {
            var key = $"job_model_{dcJobId}";
            return await cache.GetOrCreateAsync<JobModel>(key, async ce =>
            {
                var jobModel = await dataContext.GetJobByDcJobId(dcJobId) ?? throw new InvalidOperationException($"Job not found. External Job id: {dcJobId}");
                ce.Value = jobModel;
                ce.SlidingExpiration = TimeSpan.FromSeconds(120);
                return jobModel;
            });
        }

        private void SendTelemetry(JobModel job, List<JobStepModel> jobSteps)
        {
            jobSteps.Where(step => step.StartTime.HasValue && step.EndTime.HasValue)
                .ToList()
                .ForEach(step =>
                {
                    logger.LogVerbose($"Now generating telemetry for completed message {step.MessageId}, {step.MessageName}");
                    var props = new Dictionary<string, string>
                    {
                        { TelemetryKeys.MessageName, step.MessageName },
                        { TelemetryKeys.JobType, job.JobType.ToString("G")},
                        { "JobId", job.Id.ToString()},
                        { TelemetryKeys.Id, step.Id.ToString() },
                        { "MessageId",step.MessageId.ToString("N") },
                        { TelemetryKeys.ExternalJobId, job.DcJobId.ToString() },
                        { TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString() },
                        { TelemetryKeys.CollectionYear, job.CollectionYear.ToString()}
                    };
                    if (job.Ukprn != null)
                        props.Add(TelemetryKeys.Ukprn, job.Ukprn.ToString());
                    telemetry.TrackEvent("Processed Message", props, new Dictionary<string, double> { { TelemetryKeys.Duration, (step.EndTime.Value - step.StartTime.Value).TotalMilliseconds } });
                });
        }

    }
}