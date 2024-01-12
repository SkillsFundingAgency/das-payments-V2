using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public abstract class BaseJobStatusService
    {
        public IJobStorageService JobStorageService { get; }
        public IPaymentLogger Logger { get; }
        public ITelemetry Telemetry { get; }
        public IJobStatusEventPublisher EventPublisher { get; }
        public IJobServiceConfiguration Config { get; }
        protected abstract TimeSpan JobTimeoutPeriod { get; }

        protected BaseJobStatusService(IJobStorageService jobStorageService, IPaymentLogger logger, ITelemetry telemetry, IJobStatusEventPublisher eventPublisher, IJobServiceConfiguration config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            JobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            EventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        protected virtual bool IsJobTimedOut(JobModel job, CancellationToken cancellationToken)
        {
            var timedOutTime = DateTimeOffset.UtcNow;

            if (job.Status != JobStatus.InProgress || job.StartTime.Add(JobTimeoutPeriod) >= timedOutTime)
                return false;

            Logger.LogWarning($"Job {job.DcJobId} has timed out.");
            return true;
        }

        protected async Task<bool> CompleteJob(long jobId, JobStatus status, DateTimeOffset endTime, CancellationToken cancellationToken)
        {
            var job = await JobStorageService.GetJob(jobId, cancellationToken);
            if (job == null)
            {
                Logger.LogWarning($"Attempting to record completion status for job {jobId} but the job has not been persisted to database.");
                return false;
            }

            return await CompleteJob(job, status, endTime, cancellationToken);
        }

        protected virtual async Task<bool> CompleteJob(JobModel job, JobStatus status, DateTimeOffset endTime, CancellationToken cancellationToken)
        {
            job.Status = status;
            job.EndTime = endTime;
            await JobStorageService.SaveJobStatus(job.DcJobId.Value, status, endTime, cancellationToken);

            SendTelemetry(job);

            if (job.Status == JobStatus.DcTasksFailed || job.Status == JobStatus.TimedOut)
            {
                Logger.LogWarning($"Finished recording completion status of job. Job: {job.DcJobId}, status: {job.Status}, end time: {job.EndTime}");
            }
            else
            {
                Logger.LogInfo($"Finished recording completion status of job. Job: {job.DcJobId}, status: {job.Status}, end time: {job.EndTime}");
            }

            return true;
        }

        protected void SendTelemetry(JobModel job)
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
            Telemetry.TrackEvent("Finished Job", properties, metrics);
        }
    }
}