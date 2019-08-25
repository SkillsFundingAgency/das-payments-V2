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

            var inProgressMessages = await jobStorageService.GetInProgressMessageIdentifiers(cancellationToken)
                .ConfigureAwait(false);

            if (inProgressMessages.Any())
                return JobStatus.InProgress;

            var jobStatus = await jobStorageService.GetJobStatus(cancellationToken);
            if (jobStatus.jobStatus == JobStatus.InProgress)
                return JobStatus.InProgress;

            var job = await jobStorageService.GetJob(cancellationToken);
            if (job == null)
                return JobStatus.InProgress;


            job.Status = jobStatus.jobStatus;
            job.EndTime = jobStatus.endTime;
            await jobStorageService.UpdateJob(job, cancellationToken).ConfigureAwait(false);
            return job.Status;

            //TODO: move to private method
            telemetry.AddProperty(TelemetryKeys.Id, job.Id.ToString());
            telemetry.AddProperty(TelemetryKeys.JobType, job.JobType.ToString("G"));
            telemetry.AddProperty(TelemetryKeys.Ukprn, job.Ukprn?.ToString() ?? string.Empty);
            telemetry.AddProperty(TelemetryKeys.ExternalJobId, job.DcJobId?.ToString() ?? string.Empty);
            telemetry.AddProperty(TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString());
            telemetry.AddProperty(TelemetryKeys.AcademicYear, job.AcademicYear.ToString());
            telemetry.AddProperty(TelemetryKeys.Status, job.Status.ToString("G"));
            var metrics = new Dictionary<string, double>
            {
                {TelemetryKeys.Duration, (job.EndTime.Value - job.StartTime).TotalMilliseconds},
                //{TelemetryKeys.MessageCount, stepsStatus.Values.Sum()},
            };
            if (job.JobType == JobType.EarningsJob)
                metrics.Add("Learner Count", job.LearnerCount ?? 0);
            telemetry.TrackEvent("Finished Job", metrics);
            logger.LogInfo($"Finished recording completion status of job. Job: {job.Id}, status: {job.Status}, end time: {job.EndTime}");
        }
    }
}