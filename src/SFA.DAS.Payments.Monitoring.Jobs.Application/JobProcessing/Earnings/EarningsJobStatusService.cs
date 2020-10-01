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
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.Earnings
{

    public interface IEarningsJobStatusService: IJobStatusService{ }

    public class EarningsJobStatusService : JobStatusService, IEarningsJobStatusService
    {
        public EarningsJobStatusService(IJobStorageService jobStorageService, IPaymentLogger logger,
            ITelemetry telemetry, IJobStatusEventPublisher eventPublisher, IJobServiceConfiguration config)
            : base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
        }

        protected override async Task<bool> CheckSavedJobStatus(JobModel job, CancellationToken cancellationToken)
        {
            if (job.Status == JobStatus.InProgress && job.LearnerCount > 0)
                return false;

            if (job.DcJobSucceeded.HasValue && !job.DcJobSucceeded.Value && job.Status != JobStatus.DcTasksFailed)
            {
                job.Status = JobStatus.DcTasksFailed;
                await JobStorageService.SaveJobStatus(job.DcJobId.Value, JobStatus.DcTasksFailed,
                    job.StartTime, cancellationToken).ConfigureAwait(false);
            }

            if (job.Status != JobStatus.InProgress && job.DcJobSucceeded.HasValue)
            {
                Logger.LogWarning($"Job {job.DcJobId} has already finished. Status: {job.Status}");
                await EventPublisher.SubmissionFinished(job.DcJobSucceeded.Value, job.DcJobId.Value, job.Ukprn.Value,
                    job.AcademicYear, job.CollectionPeriod, job.IlrSubmissionTime.Value).ConfigureAwait(false);
                return true;
            }

            if (job.LearnerCount.HasValue && job.LearnerCount.Value == 0)
            {
                await JobStorageService.SaveJobStatus(job.DcJobId.Value, JobStatus.Completed,
                    job.StartTime, cancellationToken).ConfigureAwait(false);
            }

            return false;
        }

        protected override async Task ManageMessageStatus(long jobId, List<CompletedMessage> completedMessages,
            List<InProgressMessage> inProgressMessages,
            CancellationToken cancellationToken)
        {
            await CompleteDataLocks(jobId, completedMessages, inProgressMessages, cancellationToken)
                .ConfigureAwait(false);
            await base.ManageMessageStatus(jobId, completedMessages, inProgressMessages, cancellationToken)
                .ConfigureAwait(false);
        }

        protected override async Task<bool> CompleteJob(JobModel job, JobStatus status, DateTimeOffset endTime,
            CancellationToken cancellationToken)
        {
            status = job.DcJobSucceeded.HasValue && !job.DcJobSucceeded.Value
                ? JobStatus.DcTasksFailed
                : status;
            if (!await base.CompleteJob(job, status, endTime, cancellationToken))
                return false;

            if (!job.DcJobSucceeded.HasValue)
                return false;
            if (job.JobType == JobType.EarningsJob || job.JobType == JobType.ComponentAcceptanceTestEarningsJob)
                await EventPublisher.SubmissionFinished(status == JobStatus.Completed || status == JobStatus.CompletedWithErrors, job.DcJobId.Value, job.Ukprn.Value, job.AcademicYear, job.CollectionPeriod, job.IlrSubmissionTime.Value).ConfigureAwait(false);
            return true;
        }

        private async Task CompleteDataLocks(long jobId, List<CompletedMessage> completedMessages,
            List<InProgressMessage> inProgressMessages, CancellationToken cancellationToken)
        {
            var job = await JobStorageService.GetJob(jobId, cancellationToken).ConfigureAwait(false);
            if (job == null)
                return;

            var dataLocksMessages = new[]
            {
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
                .Where(inProgress =>
                    dataLocksMessages.Any(dlockType => inProgress.MessageName?.Contains(dlockType) ?? false))
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
            await JobStorageService.SaveDataLocksCompletionTime(job.DcJobId.Value, completionTime, cancellationToken)
                .ConfigureAwait(false);

            var properties = new Dictionary<string, string>
            {
                {TelemetryKeys.JobId, job.DcJobId.ToString()},
                {TelemetryKeys.JobType, job.JobType.ToString("G")},
                {TelemetryKeys.Ukprn, job.Ukprn?.ToString() ?? string.Empty},
                {TelemetryKeys.InternalJobId, job.Id.ToString()},
                {TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString()},
                {TelemetryKeys.AcademicYear, job.AcademicYear.ToString()},
                {TelemetryKeys.Status, job.Status.ToString("G")}
            };

            var metrics = new Dictionary<string, double>
            {
                {TelemetryKeys.Duration, (completionTime - job.StartTime).TotalMilliseconds},
                {"Learner Count", job.LearnerCount ?? 0}
            };
            Telemetry.TrackEvent("Finished DataLocks", properties, metrics);

            Logger.LogInfo(
                $"Recorded DataLocks completion time for Job: {job.DcJobId}, completion time: {completionTime}");
        }
    }
}