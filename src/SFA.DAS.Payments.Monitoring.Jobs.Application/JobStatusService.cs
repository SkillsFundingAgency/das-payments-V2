using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobStatusService
    {
        Task UpdateStatus(JobModel job, CancellationToken cancellationToken = default(CancellationToken));
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

        public async Task UpdateStatus(JobModel job, CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogDebug($"Now getting job steps status for job: {job.Id}");



            //telemetry.AddProperty(TelemetryKeys.Id, job.Id.ToString());
            //telemetry.AddProperty(TelemetryKeys.JobType, job.JobType.ToString("G"));
            //telemetry.AddProperty(TelemetryKeys.Ukprn, job.Ukprn?.ToString() ?? string.Empty);
            //telemetry.AddProperty(TelemetryKeys.ExternalJobId, job.DcJobId?.ToString() ?? string.Empty);
            //telemetry.AddProperty(TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString());
            //telemetry.AddProperty(TelemetryKeys.AcademicYear, job.AcademicYear.ToString());
            //telemetry.AddProperty(TelemetryKeys.Status, job.Status.ToString("G"));
            //var metrics = new Dictionary<string, double>
            //{
            //    {TelemetryKeys.Duration, (job.EndTime.Value - job.StartTime).TotalMilliseconds},
            //    {TelemetryKeys.MessageCount, stepsStatus.Values.Sum()},
            //};
            //if (job.JobType == JobType.EarningsJob)
            //    metrics.Add("Learner Count", job.LearnerCount ?? 0);
            //telemetry.TrackEvent("Finished Job", metrics);
            logger.LogInfo($"Finished recording completion status of job. Job: {job.Id}, status: {status}, end time: {endTime}");
        }
    }
}