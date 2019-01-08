using System;
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
            telemetry.AddProperty("JobType", job.JobType.ToString("G"));
            telemetry.AddProperty("Ukprn", job.Ukprn?.ToString() ?? string.Empty);
            telemetry.AddProperty("External Job Id", job.DcJobId?.ToString() ?? string.Empty);
            telemetry.AddProperty("CollectionPeriod", job.CollectionPeriod.ToString());
            telemetry.AddProperty("CollectionYear", job.CollectionYear.ToString());
            telemetry.TrackDuration("Finished Job", job.EndTime.Value - job.StartTime);
            logger.LogInfo($"Finished recording completion status of job. Job: {job.Id}, status: {status}, end time: {endTime}");
        }
    }
}