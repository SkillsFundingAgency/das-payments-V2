using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog.Core;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface IIlrReprocessingJobStatusService : IJobStatusService
    {
    }

    public class IlrReprocessingJobStatusService : IJobStatusService, IIlrReprocessingJobStatusService
    {
        private readonly IJobStorageService jobStorageService;
        private readonly IPaymentLogger logger;
        private readonly ITelemetry telemetry;
        private readonly IJobStatusEventPublisher eventPublisher;
        private readonly IJobServiceConfiguration config;
        private readonly IJobsDataContext dataContext;

        public IlrReprocessingJobStatusService(IJobStorageService jobStorageService, IPaymentLogger logger,
            ITelemetry telemetry, IJobStatusEventPublisher eventPublisher, IJobServiceConfiguration config,
            IJobsDataContext dataContext)
        {
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            this.eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        private void SendTelemetry(JobModel job, List<OutstandingJobResult> processingJobsPresent = null,
            List<long?> jobsWithoutSubmissionSummariesPresent = null)
        {
            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, job.DcJobId.Value.ToString() },
                { TelemetryKeys.JobType, job.JobType.ToString("G") },
                { TelemetryKeys.Ukprn, job.Ukprn?.ToString() ?? string.Empty },
                { TelemetryKeys.InternalJobId, job.DcJobId.ToString() },
                { TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString() },
                { TelemetryKeys.AcademicYear, job.AcademicYear.ToString() },
                { TelemetryKeys.Status, job.Status.ToString("G") }
            };

            if (processingJobsPresent != null)
            {
                properties.Add("InProgressJobsCount", processingJobsPresent.Count.ToString());
                properties.Add("InProgressJobsList", string.Join(", ", processingJobsPresent.Select(j => j.ToJson())));
            }
            
            if (jobsWithoutSubmissionSummariesPresent != null)
            {
                properties.Add("jobsWithoutSubmissionSummariesCount",
                    jobsWithoutSubmissionSummariesPresent.Count.ToString());
                properties.Add("jobsWithoutSubmissionSummaries",
                    string.Join(", ", jobsWithoutSubmissionSummariesPresent.Select(j => j.ToJson())));
            }

            telemetry.TrackEvent("PeriodEndStart Job Status Update", properties, new Dictionary<string, double>());
        }

        private async Task CheckAndUpdateProcessingJobsIfRunningLongerThenAverageTime(
            List<OutstandingJobResult> processingJobs,
            List<InProgressJobAverageJobCompletionTime> averageJobCompletionTimes,
            long? dcJobId,
            CancellationToken cancellationToken)
        {
            foreach (var inProgressJob in processingJobs)
            {
                var providerJobTimings = averageJobCompletionTimes.SingleOrDefault(a => a.Ukprn == inProgressJob.Ukprn);

                if (providerJobTimings == null)
                {
                    logger.LogWarning(
                        $"Unable to find Average Job Completion Times For {inProgressJob.Ukprn}, Period End Start JobId {dcJobId}");


                }

                if (inProgressJob.JobRunTimeDurationInMillisecond > (providerJobTimings.AverageJobCompletionTime ?? 0))
                {
                    logger.LogWarning(
                        $"File Processing job {inProgressJob.DcJobId} Started at {inProgressJob.StartTime}. " +
                        $"it has been running for {inProgressJob.JobRunTimeDurationInMillisecond} Millisecond which is longer then its average duration of {providerJobTimings.AverageJobCompletionTime} Millisecond, " +
                        $"now updating job status to TimedOut, Period End Start JobId {dcJobId}");
                    //TODO: Really not sure this job should be performing this update
                    //await dataContext.SaveJobStatus(inProgressJob.DcJobId.Value, JobStatus.TimedOut, DateTimeOffset.UtcNow,
                    //    cancellationToken);
                }
                else
                {
                    logger.LogDebug(
                        $"File Processing job {inProgressJob.DcJobId} Started at {inProgressJob.StartTime}. " +
                        $"it has been running for {inProgressJob.JobRunTimeDurationInMillisecond} Millisecond which is still with in its average duration of {providerJobTimings.AverageJobCompletionTime} Millisecond, " +
                        $"Period End Start JobId {dcJobId}");
                }
            }
        }

        protected async Task CompleteJob(JobModel job, JobStatus status, DateTimeOffset endTime,
            CancellationToken cancellationToken)
        {
            job.Status = status;
            job.EndTime = endTime;
            await jobStorageService.SaveJobStatus(job.DcJobId.Value, status, endTime, cancellationToken);
            await eventPublisher.PeriodEndJobFinished(job,status == JobStatus.Completed || status == JobStatus.CompletedWithErrors);
            logger.LogInfo($"Completed PeriodEnd ILR Reprocessing Job {job.DcJobId}.");
        }

        protected bool IsJobTimedOut(JobModel job)
        {
            return job.Status == JobStatus.InProgress && job.StartTime.Add(config.EarningsJobTimeout) < DateTimeOffset.UtcNow;
        }

        public async Task<bool> ManageStatus(long jobId, CancellationToken cancellationToken)
        {
            try
            {
                
                var job = await jobStorageService.GetJob(jobId, cancellationToken);
                if (job == null)
                {
                    logger.LogWarning($"ILR Reprocessing job {jobId} not found.");
                    return false;
                }

                var outstandingJobs = await dataContext.GetOutstandingOrTimedOutJobs(job, cancellationToken);

                var timedOutJobs = outstandingJobs.Where(x =>
                    (x.JobStatus == JobStatus.TimedOut ||
                     x.JobStatus == JobStatus.DcTasksFailed) &&
                    x.EndTime > job.StartTime).ToList();

                if (timedOutJobs.Any()) //fail fast
                {
                    logger.LogWarning(
                        $"Found timed out job: {timedOutJobs.FirstOrDefault().DcJobId}.");
                    await CompleteJob(job, JobStatus.CompletedWithErrors, DateTimeOffset.UtcNow, cancellationToken);
                    return true;
                }

                if (IsJobTimedOut(job))
                {
                    await CompleteJob(job, JobStatus.TimedOut, DateTimeOffset.UtcNow, cancellationToken);
                    return true;
                }

                var processingJobsPresent = outstandingJobs
                    .Where(x => x.JobStatus == JobStatus.InProgress || x.DcJobSucceeded == null).ToList();

                if (processingJobsPresent.Any())
                {
                    SendTelemetry(job, processingJobsPresent);

                    //var completionTimesForInProgressJobs =
                    //    await dataContext.GetAverageJobCompletionTimesForInProgressJobs(
                    //        processingJobsPresent.Select(p => p.Ukprn).ToList(), cancellationToken);

                    //await CheckAndUpdateProcessingJobsIfRunningLongerThenAverageTime(processingJobsPresent,
                    //    completionTimesForInProgressJobs, job.DcJobId, cancellationToken);
                    return false;
                }

                if (DateTimeOffset.UtcNow < job.StartTime.Add(config.TimeToWaitToReceivePeriodEndILRSubmissions))
                {
                    logger.LogDebug($"Waiting for jobs to be received.  Will wait until {job.StartTime.Add(config.TimeToWaitToReceivePeriodEndILRSubmissions)}, Job start time is: {job.StartTime}, Configured time to wait is {config.TimeToWaitToReceivePeriodEndILRSubmissions}");
                    return false;
                }

                var jobsWithoutSubmissionSummariesPresent = dataContext.DoSubmissionSummariesExistForJobs(outstandingJobs);

                if (jobsWithoutSubmissionSummariesPresent.Any())
                {
                    logger.LogInfo(
                        $"Still waiting for {jobsWithoutSubmissionSummariesPresent.Count} jobs to generate submission metrics. Period End ILR Reprocessing JobId {job.DcJobId}");
                    //TODO: Figure out why are we sending telemetry here???
                    SendTelemetry(job, null, jobsWithoutSubmissionSummariesPresent);

                    return false;
                }

                logger.LogDebug(
                    $"No Outstanding Jobs or jobs Without Submission Summaries during Period End ILR Reprocessing job, Now updating Period End Start job status to Completed, Period End Start JobId {job.DcJobId}");
                await CompleteJob(job, JobStatus.Completed, DateTimeOffset.UtcNow, cancellationToken);
                return true;

            }
            catch (Exception e)
            {
                logger.LogError($"Failed to manage status for ILR Reprocessing job. Error: {e.Message}",e);
                throw;
            }
        }
    }
}