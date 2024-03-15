using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd.Archiving
{
    public interface IPeriodEndArchiveStatusService : IJobStatusService
    {
    }


    public class PeriodEndArchiveStatusService : IPeriodEndArchiveStatusService
    {
        private readonly IPeriodEndArchiveConfiguration archiveConfig;
        private readonly IJobServiceConfiguration config;
        private readonly IJobStorageService jobStorageService;
        private readonly IPaymentLogger logger;
        private readonly ITelemetry telemetry;

        public PeriodEndArchiveStatusService(IJobStorageService jobStorageService, IPaymentLogger logger,
            ITelemetry telemetry, IJobServiceConfiguration config,
            IPeriodEndArchiveConfiguration archiveConfiguration)
        {
            archiveConfig = archiveConfiguration;
            this.jobStorageService = jobStorageService;
            this.logger = logger;
            this.telemetry = telemetry;
            this.config = config;
        }

        public async Task<bool> ManageStatus(long jobId, CancellationToken cancellationToken)
        {
            telemetry.TrackEvent($"PeriodEndArchiveStatusService: Now determining if job {jobId} has finished. ");

            var job = await jobStorageService.GetJob(jobId, cancellationToken);

            if (job != null)
            {
                if (await CheckSavedJobStatus(job, cancellationToken))
                    return true;

                if (await IsJobTimedOut(job, cancellationToken))
                    return true;
            }

            var archiveStatus = await CheckArchiveStatus(jobId);

            telemetry.TrackEvent(
                $"PeriodEndArchiveStatusService: Current status for jobId: ${jobId}, Status: ${archiveStatus}");
            switch (archiveStatus)
            {
                case "Started":
                case "InProgress":
                case "Queued":
                default:
                {
                    return false;
                }

                case "Succeeded":
                {
                    telemetry.TrackEvent(
                        $"ManageJobStatus JobId : {job.DcJobId}, JobType: {job.JobType} jobStatus: {JobStatus.Completed}. Now Completing job.");

                    await CompleteJob(jobId, JobStatus.Completed, DateTimeOffset.Now, cancellationToken);
                    return true;
                }
                case "Failed":
                {
                    telemetry.TrackEvent(
                        $"ManageJobStatus JobId : {job.DcJobId}, JobType: {job.JobType} jobStatus: {JobStatus.PaymentsTaskFailed}. Archiving Job Failed.");

                    await CompleteJob(jobId, JobStatus.PaymentsTaskFailed, DateTimeOffset.Now, cancellationToken);
                    return true;
                }
            }
        }

        protected Task<bool> CheckSavedJobStatus(JobModel job, CancellationToken cancellationToken)
        {
            switch (job.Status)
            {
                case JobStatus.InProgress: return Task.FromResult(false);
                case JobStatus.Completed: return Task.FromResult(true);
                case JobStatus.PaymentsTaskFailed: return Task.FromResult(false);
                case JobStatus.TimedOut: return Task.FromResult(false);
                case JobStatus.CompletedWithErrors:
                default:
                {
                    return Task.FromResult(false);
                }
            }
        }

        protected async Task<bool> IsJobTimedOut(JobModel job, CancellationToken cancellationToken)
        {
            var timedOutTime = DateTimeOffset.UtcNow;

            if (job.Status != JobStatus.InProgress || job.StartTime.Add(config.PeriodEndRunJobTimeout) >= timedOutTime)
                return false;

            var status = JobStatus.TimedOut;
            if (job.DcJobSucceeded.HasValue)
                status = job.DcJobSucceeded.Value ? JobStatus.CompletedWithErrors : JobStatus.PaymentsTaskFailed;

            logger.LogWarning(
                $"Job {job.DcJobId} has timed out. {(status != JobStatus.TimedOut ? $"but because DcJobSucceeded is {job.DcJobSucceeded}, " : "")}Setting JobStatus as {status}");

            return await CompleteJob(job, status, timedOutTime, cancellationToken);
        }

        private async Task<List<CompletedMessage>> GetCompletedMessages(long jobId,
            List<InProgressMessage> inProgressMessages, CancellationToken cancellationToken)
        {
            var completedMessages = await jobStorageService.GetCompletedMessages(jobId, cancellationToken);

            var completedItems = completedMessages
                .Where(completedMessage =>
                    inProgressMessages.Any(inProgress => inProgress.MessageId == completedMessage.MessageId)).ToList();

            return completedItems;
        }

        private async Task<bool> CompleteJob(long jobId, JobStatus status, DateTimeOffset endTime,
            CancellationToken cancellationToken)
        {
            var job = await jobStorageService.GetJob(jobId, cancellationToken);
            if (job != null) return await CompleteJob(job, status, endTime, cancellationToken);
            logger.LogWarning(
                $"Attempting to record completion status for job {jobId} but the job has not been persisted to database.");
            return false;
        }

        protected async Task<bool> CompleteJob(JobModel job, JobStatus status, DateTimeOffset endTime,
            CancellationToken cancellationToken)
        {
            job.Status = status;
            job.EndTime = endTime;
            await jobStorageService.SaveJobStatus(job.DcJobId.Value, status, endTime, cancellationToken);

            SendTelemetry(job);

            if (job.Status == JobStatus.PaymentsTaskFailed || job.Status == JobStatus.TimedOut)
                logger.LogWarning(
                    $"Finished recording completion status of job. Job: {job.DcJobId}, status: {job.Status}, end time: {job.EndTime}");
            else
                logger.LogInfo(
                    $"Finished recording completion status of job. Job: {job.DcJobId}, status: {job.Status}, end time: {job.EndTime}");

            return true;
        }

        private async Task<string> CheckArchiveStatus(long jobId)
        {
            var param = new Dictionary<string, string>
            {
                { "jobId", jobId.ToString() }
            };
            var uri = new Uri(QueryHelpers.AddQueryString(archiveConfig.ArchiveFunctionUrl, param)).ToString();

            telemetry.TrackEvent(
                $"PeriodEndArchiveStatusService: Checking current archiving status for jobId ${jobId}, Url: {uri}");
            var result =
                await new HttpClient { Timeout = TimeSpan.FromSeconds(archiveConfig.ArchiveTimeout) }.GetAsync(
                    $"{uri}");

            if (!result.IsSuccessStatusCode)
            {
                telemetry.TrackEvent(
                    $"Error in Period end archiving function for jobId ${jobId}, Reason: ${new Exception(result.ReasonPhrase)}, Content: ${result.Content}");
                return "Failed";
            }

            var content = await result.Content.ReadAsStringAsync();

            var periodEndArchiverStatusSummary = JsonConvert.DeserializeObject<ArchiveRunInformation>(content);
            return periodEndArchiverStatusSummary.Status;
        }


        private void SendTelemetry(JobModel job)
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

            var metrics = new Dictionary<string, double>
            {
                { TelemetryKeys.Duration, (job.EndTime.Value - job.StartTime).TotalMilliseconds }
            };
            if (job.JobType == JobType.EarningsJob)
                metrics.Add("Learner Count", job.LearnerCount ?? 0);
            telemetry.TrackEvent("Finished Job", properties, metrics);
        }
    }
}