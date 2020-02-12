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

    public class EarningsJobStatusService : JobStatusService, IJobStatusService
    {
        public EarningsJobStatusService(IJobStorageService jobStorageService, IPaymentLogger logger,
            ITelemetry telemetry, IJobStatusEventPublisher eventPublisher, IJobServiceConfiguration config)
            : base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
        }

        protected override async Task<bool> CheckSavedJobStatus(JobModel job, CancellationToken cancellationToken)
        {
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
            if (!await base.CompleteJob(job, status, endTime, cancellationToken))
                return false;

            if (IsPeriodEndJob(job.JobType))
            {
                await SendPeriodEndSuccessEvents(job);
            }

            if (!job.DcJobSucceeded.HasValue)
                return false;
            if (job.JobType == JobType.EarningsJob || job.JobType == JobType.ComponentAcceptanceTestEarningsJob)
                await EventPublisher.SubmissionFinished(job.DcJobSucceeded.Value, job.DcJobId.Value, job.Ukprn.Value,
                    job.AcademicYear, job.CollectionPeriod, job.IlrSubmissionTime.Value).ConfigureAwait(false);
           
            return true;
        }

        private async Task SendPeriodEndSuccessEvents(JobModel job)
        {
            if (job.JobType == JobType.PeriodEndStartJob || job.JobType == JobType.ComponentAcceptanceTestMonthEndJob)
                await EventPublisher.PeriodEndStartFinished(true, job.DcJobId.Value, job.AcademicYear,
                    job.CollectionPeriod);

            if (job.JobType == JobType.PeriodEndRunJob || job.JobType == JobType.ComponentAcceptanceTestMonthEndJob)
                await EventPublisher.PeriodEndRunFinished(true, job.DcJobId.Value, job.AcademicYear,
                    job.CollectionPeriod);

            if (job.JobType == JobType.PeriodEndStopJob || job.JobType == JobType.ComponentAcceptanceTestMonthEndJob)
                await EventPublisher.PeriodEndStopFinished(true, job.DcJobId.Value, job.AcademicYear,
                    job.CollectionPeriod);
        }


        protected override async Task<bool> IsJobTimedOut(JobModel job, CancellationToken cancellationToken)
        {
            var isJobTimedOut = await base.IsJobTimedOut(job, cancellationToken);
            if (isJobTimedOut && IsPeriodEndJob(job.JobType))
            {
                await SendPeriodEndFailureEvents(job);
            }

            return isJobTimedOut;
        }

        private async Task SendPeriodEndFailureEvents(JobModel job)
        {
            if (job.JobType == JobType.PeriodEndStartJob || job.JobType == JobType.ComponentAcceptanceTestMonthEndJob)
                await EventPublisher.PeriodEndStartFinished(false, job.DcJobId.Value, job.AcademicYear,
                    job.CollectionPeriod);

            if (job.JobType == JobType.PeriodEndRunJob || job.JobType == JobType.ComponentAcceptanceTestMonthEndJob)
                await EventPublisher.PeriodEndRunFinished(false, job.DcJobId.Value, job.AcademicYear,
                    job.CollectionPeriod);

            if (job.JobType == JobType.PeriodEndStopJob || job.JobType == JobType.ComponentAcceptanceTestMonthEndJob)
                await EventPublisher.PeriodEndStopFinished(false, job.DcJobId.Value, job.AcademicYear,
                    job.CollectionPeriod);
        }

        private bool IsPeriodEndJob(JobType jobType)
        {
            return jobType == JobType.PeriodEndStartJob ||
                   jobType == JobType.PeriodEndRunJob ||
                   jobType == JobType.PeriodEndStopJob ||
                   jobType == JobType.ComponentAcceptanceTestMonthEndJob;
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