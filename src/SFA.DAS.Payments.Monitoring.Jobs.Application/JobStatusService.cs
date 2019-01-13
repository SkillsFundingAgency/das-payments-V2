using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobStatusService
    {
        Task UpdateStatus(JobModel job, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class JobStatusService : IJobStatusService
    {
        private readonly IJobsDataContext dataContext;
        private readonly IPaymentLogger logger;
        private readonly ITelemetry telemetry;

        public JobStatusService(IJobsDataContext dataContext, IPaymentLogger logger, ITelemetry telemetry)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task UpdateStatus(JobModel job, CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogDebug($"Now getting job steps status for job: {job.Id}");
            var stepsStatus = await dataContext.GetJobStepsStatus(job.Id);
            if (stepsStatus.ContainsKey(JobStepStatus.Queued) || stepsStatus.ContainsKey(JobStepStatus.Processing))
            {
                logger.LogDebug($"Not all job steps have finished processing for job: {job.Id}.  There are still {stepsStatus.Keys.Count(key => key == JobStepStatus.Processing || key == JobStepStatus.Queued)} steps currently in progress.");
                return;
            }

            if (cancellationToken.IsCancellationRequested)
                return;
            logger.LogVerbose($"Job has finished, now getting time of last job step for job {job.Id}");
            var endTime = await dataContext.GetLastJobStepEndTime(job.Id);
            if (cancellationToken.IsCancellationRequested)
                return;
            var status = stepsStatus.Keys.Any(key => key == JobStepStatus.Failed)
                ? JobStatus.CompletedWithErrors
                : JobStatus.Completed;
            logger.LogDebug($"Got end time of last step for job: {job.Id}, time: {endTime}. Status: {status}");
            job.Status = status;
            job.EndTime = endTime;
            await dataContext.UpdateJob(job, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
            telemetry.AddProperty(TelemetryKeys.Id, job.Id.ToString());
            telemetry.AddProperty(TelemetryKeys.JobType, job.JobType.ToString("G"));
            telemetry.AddProperty(TelemetryKeys.Ukprn, job.Ukprn?.ToString() ?? string.Empty);
            telemetry.AddProperty(TelemetryKeys.ExternalJobId, job.DcJobId?.ToString() ?? string.Empty);
            telemetry.AddProperty(TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString());
            telemetry.AddProperty(TelemetryKeys.CollectionYear, job.CollectionYear.ToString());
            telemetry.AddProperty(TelemetryKeys.Status, job.Status.ToString("G"));
            var metrics = new Dictionary<string, double>
            {
                {TelemetryKeys.Duration, (job.EndTime.Value - job.StartTime).TotalMilliseconds},
                {TelemetryKeys.MessageCount, stepsStatus.Values.Sum()},
            };
            if (job.JobType == JobType.EarningsJob)
                metrics.Add("Learner Count", job.LearnerCount ?? 0);
            telemetry.TrackEvent("Finished Job", metrics);
            logger.LogInfo($"Finished recording completion status of job. Job: {job.Id}, status: {status}, end time: {endTime}");
        }
    }
}