using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Exceptions;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobMessageService
    {
        Task JobMessageCompleted(RecordJobMessageProcessingStatus jobMessageStatus, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class JobMessageService : IJobMessageService
    {
        private readonly IJobStorageService jobStorageService;
        private readonly IPaymentLogger logger;
        private readonly ITelemetry telemetry;

        public JobMessageService(IJobStorageService jobStorageService, IPaymentLogger logger, ITelemetry telemetry)
        {
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task JobMessageCompleted(RecordJobMessageProcessingStatus jobMessageStatus, CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogVerbose($"Now recording completion of message processing.  Job Id: {jobMessageStatus.JobId}, Message id: {jobMessageStatus.Id}.");
            var messageIds = new List<Guid> { jobMessageStatus.Id };
            messageIds.AddRange(jobMessageStatus.GeneratedMessages.Select(msg => msg.MessageId));

            var jobMessages = await jobStorageService.GetJobMessages(messageIds, cancellationToken).ConfigureAwait(false);
            var completedJobMessage = jobMessages.FirstOrDefault(msg => msg.MessageId == jobMessageStatus.Id);
            if (completedJobMessage == null)
            {
                completedJobMessage = new JobStepModel
                {
                    MessageId = jobMessageStatus.Id,
                    MessageName = jobMessageStatus.MessageName,
                };
                jobMessages.Add(completedJobMessage);
            }

            completedJobMessage.EndTime = jobMessageStatus.EndTime;
            completedJobMessage.Status = jobMessageStatus.Succeeded ? JobStepStatus.Completed : JobStepStatus.Failed;

            foreach (var generatedMessage in jobMessageStatus.GeneratedMessages)
            {
                var jobMessage = jobMessages.FirstOrDefault(msg => msg.MessageId == jobMessageStatus.Id);
                if (jobMessage == null)
                {
                    jobMessage = new JobStepModel
                    {
                        MessageId = jobMessageStatus.Id,
                        MessageName = jobMessageStatus.MessageName,
                    };
                    jobMessages.Add(jobMessage);
                }

                jobMessage.EndTime = jobMessageStatus.EndTime;
                jobMessage.Status = jobMessageStatus.Succeeded ? JobStepStatus.Completed : JobStepStatus.Failed;

            }

            await jobStorageService.StoreJobMessages(jobMessages, cancellationToken);
            //var jobSteps = await dataContext.GetJobSteps(messageIds);
            //var job = await GetJob(jobMessageStatus.JobId);
            //var jobId = job.Id;
            //var completedStep = jobSteps.FirstOrDefault(step => step.MessageId == jobMessageStatus.Id);
            //if (completedStep == null)
            //{
            //    logger.LogDebug($"Recording completion of job step before the start time of the step has been recorded. Job: {jobId}, Message Id: {jobMessageStatus.Id}");
            //    completedStep = new JobStepModel
            //    {
            //        JobId = jobId,
            //        MessageId = jobMessageStatus.Id,
            //        MessageName = jobMessageStatus.MessageName,

            //    };
            //    jobSteps.Add(completedStep);
            //}
            //completedStep.Status = jobMessageStatus.Succeeded ? JobStepStatus.Completed : JobStepStatus.Failed;
            //completedStep.EndTime = jobMessageStatus.EndTime;

            //foreach (var generatedMessage in jobMessageStatus.GeneratedMessages)
            //{
            //    var jobStep = jobSteps.FirstOrDefault(step => step.MessageId == generatedMessage.MessageId);
            //    if (jobStep == null)
            //    {
            //        jobStep = new JobStepModel
            //        {
            //            JobId = jobId,
            //            MessageId = generatedMessage.MessageId,
            //            MessageName = generatedMessage.MessageName,
            //            Status = JobStepStatus.Queued
            //        };
            //        jobSteps.Add(jobStep);
            //    }
            //    else
            //    {
            //        logger.LogDebug($"Updating job step to record the start time. Job id: {jobId}, Message id: {generatedMessage.MessageId}");
            //    }

            //    jobStep.StartTime = generatedMessage.StartTime;
            //    jobStep.ParentMessageId = jobMessageStatus.Id;
            //}
            //logger.LogVerbose("Now saving updated job steps.");
            //await dataContext.SaveJobSteps(jobSteps);
            logger.LogDebug("Finished saving updated job steps to db.");
            //SendTelemetry(job, jobSteps);
            logger.LogInfo($"Recorded completion of message processing.  Job Id: {jobMessageStatus.JobId}, Message id: {jobMessageStatus.Id}.");
        }

        //private async Task<JobModel> GetJob(long dcJobId)
        //{
        //    var key = $"job_model_{dcJobId}";
        //    return await cache.GetOrCreateAsync<JobModel>(key, async ce =>
        //    {
        //        var jobModel = await dataContext.GetJobByDcJobId(dcJobId) ?? throw new DcJobNotFoundException(dcJobId);
        //        ce.Value = jobModel;
        //        ce.SlidingExpiration = TimeSpan.FromSeconds(120);
        //        return jobModel;
        //    });
        //}

        //private void SendTelemetry(JobModel job, List<JobStepModel> jobSteps)
        //{
        //    jobSteps.Where(step => step.StartTime.HasValue && step.EndTime.HasValue)
        //        .ToList()
        //        .ForEach(step =>
        //        {
        //            logger.LogVerbose($"Now generating telemetry for completed message {step.MessageId}, {step.MessageName}");
        //            var props = new Dictionary<string, string>
        //            {
        //                { TelemetryKeys.MessageName, step.MessageName },
        //                { TelemetryKeys.JobType, job.JobType.ToString("G")},
        //                { "JobId", job.Id.ToString()},
        //                { TelemetryKeys.Id, step.Id.ToString() },
        //                { "MessageId",step.MessageId.ToString("N") },
        //                { TelemetryKeys.ExternalJobId, job.DcJobId.ToString() },
        //                { TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString() },
        //                { TelemetryKeys.AcademicYear, job.AcademicYear.ToString()}
        //            };
        //            if (job.Ukprn != null)
        //                props.Add(TelemetryKeys.Ukprn, job.Ukprn.ToString());
        //            telemetry.TrackEvent("Processed Message", props, new Dictionary<string, double> { { TelemetryKeys.Duration, (step.EndTime.Value - step.StartTime.Value).TotalMilliseconds } });
        //        });
        //}

    }
}