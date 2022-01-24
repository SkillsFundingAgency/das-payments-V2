using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface IPeriodEndStartJobStatusService : IPeriodEndJobStatusService { }
    public class PeriodEndStartJobStatusService : PeriodEndJobStatusService, IPeriodEndStartJobStatusService
    {
        private readonly IJobsDataContext context;

        public PeriodEndStartJobStatusService(
            IJobStorageService jobStorageService,
            IPaymentLogger logger,
            ITelemetry telemetry,
            IJobStatusEventPublisher eventPublisher,
            IJobServiceConfiguration config,
            IJobsDataContext context)
            : base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<(bool IsComplete, JobStatus? OverriddenJobStatus, DateTimeOffset? completionTime)> PerformAdditionalJobChecks(JobModel job, CancellationToken cancellationToken)
        {
            var outstandingJobs = await context.GetOutstandingOrTimedOutJobs(job, cancellationToken);

            var timeoutsPresent = outstandingJobs.Where(x =>
                (x.JobStatus == JobStatus.TimedOut ||
                 x.JobStatus == JobStatus.DcTasksFailed) &&
                x.EndTime > job.StartTime).ToList();

            if (timeoutsPresent.Any()) //fail fast
            {
                Logger.LogWarning($"{timeoutsPresent.Count} File Processing jobs {string.Join(" ,", timeoutsPresent.Select(j => j.DcJobId))} with TimedOut or DcTasksFailed and job EndTime after Period-End-Start Job Present. " +
                                  $"now updating job status to CompletedWithErrors, Period End Start JobId {job.DcJobId}");

                return (true, JobStatus.CompletedWithErrors, outstandingJobs.Max(x => x.EndTime));
            }

            var processingJobsPresent = outstandingJobs.Where(x => x.JobStatus == JobStatus.InProgress || x.DcJobSucceeded == null).ToList();

            if (processingJobsPresent.Any())
            {
                SendTelemetry(job, processingJobsPresent);

                var completionTimesForInProgressJobs = await context.GetAverageJobCompletionTimesForInProgressJobs(processingJobsPresent.Select(p => p.Ukprn).ToList(), cancellationToken);

                await CheckAndUpdateProcessingJobsIfRunningLongerThenAverageTime(processingJobsPresent, completionTimesForInProgressJobs, job.DcJobId, cancellationToken);

                return (false, null, null);
            }

            var jobsWithoutSubmissionSummariesPresent = context.DoSubmissionSummariesExistForJobs(outstandingJobs);

            if (jobsWithoutSubmissionSummariesPresent.Any())
            {
                Logger.LogDebug($"{jobsWithoutSubmissionSummariesPresent.Count} File Processing jobs without Submission Summaries Present during Period End Start job, Period End Start JobId {job.DcJobId}");

                SendTelemetry(job, null, jobsWithoutSubmissionSummariesPresent);

                return (false, null, null);
            }

            Logger.LogDebug($"No Outstanding Jobs or jobs Without Submission Summaries during Period End Start job, Now updating Period End Start job status to Completed, Period End Start JobId {job.DcJobId}");

            return (true, null, DateTimeOffset.UtcNow);
        }

        private void SendTelemetry(JobModel job, List<OutstandingJobResult> processingJobsPresent = null, List<long?> jobsWithoutSubmissionSummariesPresent = null)
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

            if (processingJobsPresent != null)
            {
                properties.Add("InProgressJobsCount", processingJobsPresent.Count.ToString());
                properties.Add("InProgressJobsList", string.Join(", ", processingJobsPresent.Select(j => j.ToJson())));
            }


            if (jobsWithoutSubmissionSummariesPresent != null)
            {
                properties.Add("jobsWithoutSubmissionSummariesCount", jobsWithoutSubmissionSummariesPresent.Count.ToString());
                properties.Add("jobsWithoutSubmissionSummaries", string.Join(", ", jobsWithoutSubmissionSummariesPresent.Select(j => j.ToJson())));
            }

            Telemetry.TrackEvent("PeriodEndStart Job Status Update", properties, new Dictionary<string, double>());
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

                if (providerJobTimings == null) throw new InvalidOperationException($"Unable to find Average Job Completion Times For {inProgressJob.Ukprn}, Period End Start JobId {dcJobId}");

                if (inProgressJob.JobRunTimeDurationInMillisecond > (providerJobTimings.AverageJobCompletionTime ?? 0))
                {
                    Logger.LogWarning($"File Processing job {inProgressJob.DcJobId} Started at {inProgressJob.StartTime}. " +
                                      $"it has been running for {inProgressJob.JobRunTimeDurationInMillisecond} Millisecond which is longer then its average duration of {providerJobTimings.AverageJobCompletionTime} Millisecond, " +
                                      $"now updating job status to TimedOut, Period End Start JobId {dcJobId}");

                    await context.SaveJobStatus(inProgressJob.DcJobId.Value, JobStatus.TimedOut, DateTimeOffset.UtcNow, cancellationToken);
                }
                else
                {
                    Logger.LogDebug($"File Processing job {inProgressJob.DcJobId} Started at {inProgressJob.StartTime}. " +
                                    $"it has been running for {inProgressJob.JobRunTimeDurationInMillisecond} Millisecond which is still with in its average duration of {providerJobTimings.AverageJobCompletionTime} Millisecond, " +
                                    $"Period End Start JobId {dcJobId}");
                }
            }
        }
    }
}
