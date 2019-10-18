using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public interface IEarningsJobService
    {
        Task RecordNewJob(RecordEarningsJob earningsJobRequest, CancellationToken cancellationToken = default(CancellationToken));
        Task RecordNewJobAdditionalMessages(RecordEarningsJobAdditionalMessages earningsJobRequest, CancellationToken cancellationToken);
        Task RecordDcJobCompleted(long jobId, bool succeeded, CancellationToken cancellationToken);
    }

    public class EarningsJobService : IEarningsJobService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobStorageService jobStorageService;
        private readonly ITelemetry telemetry;

        public EarningsJobService(IPaymentLogger logger, IJobStorageService jobStorageService, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task RecordNewJob(RecordEarningsJob earningsJobRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogDebug($"Now recording new provider earnings job.  Job Id: {earningsJobRequest.JobId}, Ukprn: {earningsJobRequest.Ukprn}.");
            var jobDetails = new JobModel
            {
                JobType = JobType.EarningsJob,
                StartTime = earningsJobRequest.StartTime,
                CollectionPeriod = earningsJobRequest.CollectionPeriod,
                AcademicYear = earningsJobRequest.CollectionYear,
                Ukprn = earningsJobRequest.Ukprn,
                DcJobId = earningsJobRequest.JobId,
                IlrSubmissionTime = earningsJobRequest.IlrSubmissionTime,
                Status = JobStatus.InProgress,
                LearnerCount = earningsJobRequest.LearnerCount
            };
            var isNewJob = await jobStorageService.StoreNewJob(jobDetails, CancellationToken.None);
            logger.LogDebug($"Finished storing new job: {jobDetails.Id} for dc job id: {jobDetails.DcJobId}, Ukprn: {earningsJobRequest.Ukprn}. Now storing the job messages.");
            await RecordJobInProgressMessages(earningsJobRequest.JobId,earningsJobRequest.GeneratedMessages,cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Finished storing new job messages for job: {jobDetails.Id} for dc job id: {jobDetails.DcJobId}, Ukprn: {earningsJobRequest.Ukprn}.");
            if (isNewJob)
                SendTelemetry(earningsJobRequest, jobDetails);
            logger.LogInfo($"Finished saving the job info.  DC Job Id: {earningsJobRequest.JobId}, Ukprn: {earningsJobRequest.Ukprn}.");
        }

        private async Task RecordJobInProgressMessages(long jobId, List<GeneratedMessage> generatedMessages, CancellationToken cancellationToken)
        {
            await jobStorageService.StoreInProgressMessages(jobId,
                generatedMessages.Select(message => new InProgressMessage
                {
                    MessageId = message.MessageId,
                    JobId = jobId,
                    MessageName = message.MessageName
                }).ToList(), cancellationToken);
        }

        public async Task RecordNewJobAdditionalMessages(RecordEarningsJobAdditionalMessages earningsJobRequest, CancellationToken cancellationToken)
        {
            await RecordJobInProgressMessages(earningsJobRequest.JobId, earningsJobRequest.GeneratedMessages, cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Finished storing new job messages for job: {earningsJobRequest.JobId}");
        }

        public async Task RecordDcJobCompleted(long jobId, bool succeeded, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Now storing the completion status of the submission job. Id: {jobId}, succeeded: {succeeded}");
            await jobStorageService.StoreDcJobStatus(jobId, succeeded, cancellationToken).ConfigureAwait(false);
            logger.LogInfo($"Finished storing the completion status of the submission job. Id: {jobId}, succeeded: {succeeded}");
        }


        private void SendTelemetry(RecordEarningsJob startedEvent, JobModel jobDetails)
        {
            telemetry.TrackEvent("Started Job",
                new Dictionary<string, string>
                {
                    { "JobType", JobType.EarningsJob.ToString("G") },
                    { "Ukprn", startedEvent.Ukprn.ToString() },
                    { "Id", jobDetails.Id.ToString() },
                    { "ExternalJobId", startedEvent.JobId.ToString() },
                    { "CollectionPeriod", startedEvent.CollectionPeriod.ToString() },
                    { "CollectionYear", startedEvent.CollectionYear.ToString() }
                },
                new Dictionary<string, double> { { "LearnerCount", startedEvent.LearnerCount } }
            );
        }
    }
}