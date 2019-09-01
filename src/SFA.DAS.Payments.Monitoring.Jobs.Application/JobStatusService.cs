using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobStatusService
    {
        Task<JobStatus> ManageStatus(CancellationToken cancellationToken = default(CancellationToken));
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

        public async Task<JobStatus> ManageStatus(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!await HasFinished(cancellationToken))
                return JobStatus.InProgress;

            var jobStatus = await jobStorageService.GetJobStatus(cancellationToken);
            if (jobStatus.jobStatus == JobStepStatus.Processing)
                return JobStatus.InProgress;

            var job = await jobStorageService.GetJob(cancellationToken);
            if (job == null)
                return JobStatus.InProgress;

            job.Status = jobStatus.jobStatus == JobStepStatus.Completed ? JobStatus.Completed : JobStatus.CompletedWithErrors;
            job.EndTime = jobStatus.endTime;
            await jobStorageService.UpdateJob(job, cancellationToken).ConfigureAwait(false);

            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.Id, job.Id.ToString()},
                { TelemetryKeys.JobType, job.JobType.ToString("G")},
                { TelemetryKeys.Ukprn, job.Ukprn?.ToString() ?? string.Empty},
                { TelemetryKeys.ExternalJobId, job.DcJobId?.ToString() ?? string.Empty},
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
            logger.LogInfo($"Finished recording completion status of job. Job: {job.Id}, status: {job.Status}, end time: {job.EndTime}");
            return job.Status;
        }

        public async Task<bool> HasFinished(CancellationToken cancellationToken)
        {
            if ((await jobStorageService.GetInProgressMessageIdentifiers(cancellationToken).ConfigureAwait(false)).Any())
                return false;

            if ((await jobStorageService.GetCompletedMessageIdentifiers(cancellationToken).ConfigureAwait(false)).Any())
                return false;

            var jobStatus = await jobStorageService.GetJobStatus(cancellationToken);
            if (jobStatus.jobStatus == JobStepStatus.Processing)
                return false;

            var job = await jobStorageService.GetJob(cancellationToken);
            if (job == null)
                return false;

            return true;
        }
    }
}