using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public interface IJobStatusService
    {
        Task<bool> ManageStatus(long jobId, CancellationToken cancellationToken);
    }

    public abstract class JobStatusService : BaseJobStatusService, IJobStatusService
    {
        public IJobServiceConfiguration Config { get; }
        protected IJobStorageService JobStorageService { get; }
        protected IPaymentLogger Logger { get; }
        protected ITelemetry Telemetry { get; }
        protected IJobStatusEventPublisher EventPublisher { get; }

        protected JobStatusService(IJobStorageService jobStorageService, IPaymentLogger logger, ITelemetry telemetry, IJobStatusEventPublisher eventPublisher, IJobServiceConfiguration config): base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
        }

        protected virtual Task<bool> CheckSavedJobStatus(JobModel job, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public virtual async Task<bool> ManageStatus(long jobId, CancellationToken cancellationToken)
        {
            Logger.LogInfo($"Now determining if job {jobId} has finished. ");

            var job = await JobStorageService.GetJob(jobId, cancellationToken);

            if (job != null)
            {
                if (await CheckSavedJobStatus(job, cancellationToken))
                    return true;

                if (IsJobTimedOut(job, cancellationToken))
                {
                    await CompleteJob(job, job.DcJobSucceeded.HasValue && !job.DcJobSucceeded.Value ? JobStatus.DcTasksFailed : JobStatus.TimedOut, DateTimeOffset.UtcNow, cancellationToken);
                    return true;
                }
            }

            var additionalJobChecksResult = await PerformAdditionalJobChecks(job, cancellationToken);
            if (!additionalJobChecksResult.IsComplete)
            {
                return false;
            }

            var inProgressMessages = await JobStorageService.GetInProgressMessages(jobId, cancellationToken);
            var completedItems = await GetCompletedMessages(jobId, inProgressMessages, cancellationToken);

            Logger.LogInfo($"ManageJobStatus JobId : {job.DcJobId}, JobType: {job.JobType} Inprogress count: {inProgressMessages.Count}, completed count: {completedItems.Count}.");

            if (!completedItems.Any())
            {
                Logger.LogVerbose($"ManageJobStatus JobId : {job.DcJobId}, JobType: {job.JobType} Inprogress count: {inProgressMessages.Count}, completed count: {completedItems.Count}. Found no completed messages.");
                return false;
            }

            cancellationToken.ThrowIfCancellationRequested();

            await ManageMessageStatus(jobId, completedItems, inProgressMessages, cancellationToken);

            var currentJobStatus = await UpdateJobStatus(jobId, completedItems, cancellationToken);

            if (!inProgressMessages.All(inProgress => completedItems.Any(item => item.MessageId == inProgress.MessageId)))
            {
                Telemetry.TrackEvent(
                    $"ManageJobStatus JobId : {job.DcJobId}, JobType: {job.JobType} Inprogress count: {inProgressMessages.Count}, completed count: {completedItems.Count}. Cannot set status for job.",
                    new Dictionary<string, string> { { "In-Progress-messageIds", string.Join(", ", inProgressMessages.Select(j => j.MessageId.ToString())) } },
                    new Dictionary<string, double> { { "Inprogress count", inProgressMessages.Count }, { "completed count", completedItems.Count } }
                );
                return false;
            }

            var jobStatus = additionalJobChecksResult.OverriddenJobStatus ?? (currentJobStatus.hasFailedMessages ? JobStatus.CompletedWithErrors : JobStatus.Completed);
            var endTime =
                (additionalJobChecksResult.completionTime.HasValue &&
                 additionalJobChecksResult.completionTime > currentJobStatus.endTime.Value)
                    ? additionalJobChecksResult.completionTime.Value
                    : currentJobStatus.endTime.Value;

            Telemetry.TrackEvent($"ManageJobStatus JobId : {job.DcJobId}, JobType: {job.JobType} jobStatus: {jobStatus}, endTime: {endTime}, Inprogress count: {inProgressMessages.Count}, completed count: {completedItems.Count}. Now Completing job.");

            return await CompleteJob(jobId, jobStatus, endTime, cancellationToken);
        }

        public virtual Task<(bool IsComplete, JobStatus? OverriddenJobStatus, DateTimeOffset? completionTime)> PerformAdditionalJobChecks(JobModel job, CancellationToken cancellationToken)
        {
            return Task.FromResult((true, (JobStatus?)null, (DateTimeOffset?)null));
        }

        protected virtual async Task ManageMessageStatus(long jobId, List<CompletedMessage> completedMessages, List<InProgressMessage> inProgressMessages, CancellationToken cancellationToken)
        {
            await JobStorageService.RemoveInProgressMessages(jobId, completedMessages.Select(item => item.MessageId).ToList(), cancellationToken);

            await JobStorageService.RemoveCompletedMessages(jobId, completedMessages.Select(item => item.MessageId).ToList(), cancellationToken);
        }

        private async Task<List<CompletedMessage>> GetCompletedMessages(long jobId, List<InProgressMessage> inProgressMessages, CancellationToken cancellationToken)
        {
            var completedMessages = await JobStorageService.GetCompletedMessages(jobId, cancellationToken);

            var completedItems = completedMessages
                .Where(completedMessage => inProgressMessages.Any(inProgress => inProgress.MessageId == completedMessage.MessageId)).ToList();

            return completedItems;
        }

        protected async Task<(bool hasFailedMessages, DateTimeOffset? endTime)> UpdateJobStatus(long jobId, List<CompletedMessage> completedItems, CancellationToken cancellationToken)
        {
            var statusChanged = false;
            var currentJobStatus = await JobStorageService.GetJobStatus(jobId, cancellationToken);
            var completedItemsEndTime = completedItems.Max(item => item.CompletedTime);
            if (!currentJobStatus.endTime.HasValue || completedItemsEndTime > currentJobStatus.endTime)
            {
                currentJobStatus.endTime = completedItemsEndTime;
                statusChanged = true;
            }

            if (currentJobStatus.hasFailedMessages || completedItems.Any(item => !item.Succeeded))
            {
                currentJobStatus.hasFailedMessages = true;
                statusChanged = true;
            }

            if (statusChanged)
            {
                Logger.LogInfo($"Detected change in job status for job: {jobId}. Has failed messages: {currentJobStatus.hasFailedMessages}, End time: {currentJobStatus.endTime}");
                await JobStorageService.StoreJobStatus(jobId, currentJobStatus.hasFailedMessages, currentJobStatus.endTime, cancellationToken);
            }

            return currentJobStatus;
        }
    }
}