using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public interface IJobStatusService
    {
        Task<JobStatus> ManageStatus(long jobId, CancellationToken cancellationToken);
        Task StopJob();
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

        public async Task<JobStatus> ManageStatus(long jobId,  CancellationToken cancellationToken)
        {
            var inProgressMessages = await jobStorageService.GetInProgressMessageIdentifiers(jobId, cancellationToken)
                .ConfigureAwait(false);
            var completedMessages = await jobStorageService.GetCompletedMessages(jobId, cancellationToken)
                .ConfigureAwait(false);

            var completedItems = completedMessages
                .Where(completedMessage => inProgressMessages.Contains(completedMessage.MessageId)).ToList();

            cancellationToken.ThrowIfCancellationRequested();

            await jobStorageService.RemoveInProgressMessageIdentifiers(jobId, completedItems.Select(item => item.MessageId).ToList(), cancellationToken)
                .ConfigureAwait(false);
            await jobStorageService.RemoveCompletedMessages(jobId, completedItems.Select(item => item.MessageId).ToList(), cancellationToken)
                .ConfigureAwait(false);

            if (!inProgressMessages.TrueForAll(messageId => completedItems.Any(item => item.MessageId == messageId)))
                return JobStatus.InProgress;



            return JobStatus.Completed;


            //var jobStatus = await jobStorageService.GetJobStatus(cancellationToken);
            //if (jobStatus.jobStatus == JobStepStatus.Processing)
            //    return JobStatus.InProgress;

            //var job = await jobStorageService.GetJob(cancellationToken);
            //if (job == null)
            //    return JobStatus.InProgress;

            //job.Status = jobStatus.jobStatus == JobStepStatus.Completed ? JobStatus.Completed : JobStatus.CompletedWithErrors;
            //job.EndTime = jobStatus.endTime;
            //await jobStorageService.UpdateJob(job, cancellationToken).ConfigureAwait(false);

            //SendTelemetry(job);
            //logger.LogInfo($"Finished recording completion status of job. Job: {job.Id}, status: {job.Status}, end time: {job.EndTime}");
            //return job.Status;
        }

        private void SendTelemetry(JobModel job)
        {
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
        }

        public async Task StopJob()
        {
            //var job = await jobStorageService.GetJob(CancellationToken.None);
            //if (job == null)
            //{
            //    logger.LogError("Cannot stop job as job has started.");  //TODO: should really be exception but not sure how runtime deals with exceptions in callbacks
            //    return; 
            //}

            //job.Status = JobStatus.TimedOut;
            //job.EndTime = DateTimeOffset.UtcNow;
            //await jobStorageService.UpdateJob(job, CancellationToken.None).ConfigureAwait(false);
            //var properties = new Dictionary<string, string>
            //{
            //    { TelemetryKeys.Id, job.Id.ToString()},
            //    { TelemetryKeys.JobType, job.JobType.ToString("G")},
            //    { TelemetryKeys.Ukprn, job.Ukprn?.ToString() ?? string.Empty},
            //    { TelemetryKeys.ExternalJobId, job.DcJobId?.ToString() ?? string.Empty},
            //    { TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString()},
            //    { TelemetryKeys.AcademicYear, job.AcademicYear.ToString()},
            //    { TelemetryKeys.Status, job.Status.ToString("G")}
            //};

            //var metrics = new Dictionary<string, double>
            //{
            //    { TelemetryKeys.Duration, (job.EndTime.Value - job.StartTime).TotalMilliseconds},
            //};
            //if (job.JobType == JobType.EarningsJob)
            //    metrics.Add("Learner Count", job.LearnerCount ?? 0);
            //telemetry.TrackEvent("Job Timed Out", properties, metrics);
        }
    }
}