using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public interface IJobStatusService
    {
        Task<JobStatus> ManageStatus(long jobId, CancellationToken cancellationToken);
    }

    public class JobStatusService : IJobStatusService
    {
        private readonly IJobStorageService jobStorageService;
        private readonly IPaymentLogger logger;
        private readonly ITelemetry telemetry;

        public JobStatusService(IJobStorageService jobStorageService, IPaymentLogger logger, ITelemetry telemetry)
        {
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task<JobStatus> ManageStatus(long jobId, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Now determining if job {jobId} has finished.");
            var job = await jobStorageService.GetJob(jobId, cancellationToken).ConfigureAwait(false);
            if (job != null )
            {
                if (job.Status != JobStatus.InProgress)
                {
                    logger.LogWarning($"Job {jobId} has already finished. Status: {job.Status}");
                    return job.Status;
                }

                if ((job.JobType == JobType.EarningsJob || job.JobType == JobType.ComponentAcceptanceTestEarningsJob) && job.LearnerCount.HasValue && job.LearnerCount.Value == 0)
                {
                    await jobStorageService.SaveJobStatus(jobId,
                        JobStatus.Completed,
                        job.StartTime, cancellationToken).ConfigureAwait(false);
                }
            }

            var inProgressMessages = await jobStorageService.GetInProgressMessages(jobId, cancellationToken)
                .ConfigureAwait(false);
            var completedItems = await GetCompletedMessages(jobId, inProgressMessages, cancellationToken).ConfigureAwait(false);
            if (!completedItems.Any())
            {
                logger.LogVerbose($"Found no completed messages for job: {jobId}");
                return JobStatus.InProgress;
            }

            await CompleteDataLocks(jobId, completedItems, inProgressMessages, cancellationToken)
                .ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            await jobStorageService.RemoveInProgressMessages(jobId, completedItems.Select(item => item.MessageId).ToList(), cancellationToken)
                .ConfigureAwait(false);
            await jobStorageService.RemoveCompletedMessages(jobId, completedItems.Select(item => item.MessageId).ToList(), cancellationToken)
                .ConfigureAwait(false);

            var currentJobStatus =
                await UpdateJobStatus(jobId, completedItems, cancellationToken).ConfigureAwait(false);

            if (!inProgressMessages.TrueForAll(inProgress => completedItems.Any(item => item.MessageId == inProgress.MessageId)))
            {
                logger.LogDebug($"Found in progress messages for job id: {jobId}.  Cannot set status for job.");
                return JobStatus.InProgress;
            }

            job = await jobStorageService.GetJob(jobId, cancellationToken);
            if (job == null)
            {
                logger.LogWarning($"Attempting to record completion status for job {jobId} but the job has not been persisted to database.");
                return JobStatus.InProgress;
            }

            job.Status = currentJobStatus.hasFailedMessages ? JobStatus.CompletedWithErrors : JobStatus.Completed;
            job.EndTime = currentJobStatus.endTime;
            await jobStorageService.SaveJobStatus(jobId,
                currentJobStatus.hasFailedMessages ? JobStatus.CompletedWithErrors : JobStatus.Completed,
                currentJobStatus.endTime.Value, cancellationToken).ConfigureAwait(false);

            SendTelemetry(job);
            logger.LogInfo($"Finished recording completion status of job. Job: {job.Id}, status: {job.Status}, end time: {job.EndTime}");
            return job.Status;
        }


        private async Task<List<CompletedMessage>> GetCompletedMessages(long jobId, List<InProgressMessage> inProgressMessages, CancellationToken cancellationToken)
        {
            var completedMessages = await jobStorageService.GetCompletedMessages(jobId, cancellationToken)
                .ConfigureAwait(false);

            var completedItems = completedMessages
                .Where(completedMessage => inProgressMessages.Any(inProgress => inProgress.MessageId == completedMessage.MessageId)).ToList();

            return completedItems;
        }

        private async Task<(bool hasFailedMessages, DateTimeOffset? endTime)> UpdateJobStatus(long jobId, List<CompletedMessage> completedItems,
            CancellationToken cancellationToken)
        {
            var statusChanged = false;
            var currentJobStatus = await jobStorageService.GetJobStatus(jobId, cancellationToken).ConfigureAwait(false);
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
                logger.LogVerbose($"Detected change in job status for job: {jobId}. Has failed messages: {currentJobStatus.hasFailedMessages}, End time: {currentJobStatus.endTime}");
                await jobStorageService.StoreJobStatus(jobId, currentJobStatus.hasFailedMessages, currentJobStatus.endTime, cancellationToken)
                    .ConfigureAwait(false);
            }

            return currentJobStatus;
        }

        private async Task CompleteDataLocks(long jobId, List<CompletedMessage> completedMessages, List<InProgressMessage> inProgressMessages, CancellationToken cancellationToken)
        {
            var dataLocksMessages = new[] {
                nameof(FunctionalSkillEarningFailedDataLockMatching),
                nameof(PayableFunctionalSkillEarningEvent),
                nameof(PayableEarningEvent),
                nameof(EarningFailedDataLockMatching),
                nameof(ProcessLearnerCommand),
                nameof(Act1FunctionalSkillEarningsEvent),
                nameof(ApprenticeshipContractType1EarningEvent),
                nameof(EarningFailedDataLockMatching),
            };
            var inProgressDataLocks = inProgressMessages
                .Where(inProgress => dataLocksMessages.Any(dlockType => inProgress.MessageName?.Contains(dlockType) ?? false))
                .ToList();
            if (!inProgressDataLocks.Any())
                return;
            if (!inProgressDataLocks.All(inProgress =>
                completedMessages.Any(completed => completed.MessageId == inProgress.MessageId)))
                return;
            var completionTime = completedMessages
                .Where(completed =>
                    inProgressDataLocks.Exists(inProgress => inProgress.MessageId == completed.MessageId))
                .Max(completed => completed.CompletedTime);
            await jobStorageService.SaveDataLocksCompletionTime(jobId, completionTime, cancellationToken)
                .ConfigureAwait(false);

            //var properties = new Dictionary<string, string>
            //{
            //    { TelemetryKeys.JobId, jobId.ToString()},
            //    { TelemetryKeys.JobType, JobType.EarningsJob.ToString("G")},
            //    { TelemetryKeys.Ukprn, job.Ukprn?.ToString() ?? string.Empty},
            //    { TelemetryKeys.InternalJobId, job.DcJobId.ToString()},
            //    { TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString()},
            //    { TelemetryKeys.AcademicYear, job.AcademicYear.ToString()},
            //    { TelemetryKeys.Status, job.Status.ToString("G")}
            //};

            //var metrics = new Dictionary<string, double>
            //{
            //    { TelemetryKeys.Duration, (job.EndTime.Value - job.StartTime).TotalMilliseconds},
            //};
            //if (job.JobType == JobType.EarningsJob)
            //    metrics.Add("Learner Count", job.LearnerCount ?? 0);
            //telemetry.TrackEvent("Finished DataLocks", properties, metrics);

            logger.LogInfo($"Recorded DataLocks completion time for Job: {jobId}, completion time: {completionTime}");
        }

        private void SendTelemetry(JobModel job)
        {
            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, job.DcJobId.Value.ToString()},
                { TelemetryKeys.JobType, job.JobType.ToString("G")},
                { TelemetryKeys.Ukprn, job.Ukprn?.ToString() ?? string.Empty},
                { TelemetryKeys.InternalJobId, job.DcJobId.ToString()},
                { TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString()},
                { TelemetryKeys.AcademicYear, job.AcademicYear.ToString()},
                { TelemetryKeys.Status, job.Status.ToString("G")}
            };

            var metrics = new Dictionary<string, double>
            {
                { TelemetryKeys.Duration, (job.EndTime.Value - job.StartTime).TotalMilliseconds},
            };
            if (job.JobType == JobType.EarningsJob)
                metrics.Add("Learner Count", job.LearnerCount ?? 0);
            telemetry.TrackEvent("Finished Job", properties, metrics);
        }



    }
}