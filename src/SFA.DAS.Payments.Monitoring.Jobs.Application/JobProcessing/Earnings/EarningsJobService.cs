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


    public class EarningsJobService : JobService, IEarningsJobService
    {
        public EarningsJobService(IPaymentLogger logger, IJobStorageService jobStorageService, ITelemetry telemetry)
        :base(logger,jobStorageService, telemetry)
        {
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
            await RecordNewJob(jobDetails, earningsJobRequest.GeneratedMessages, cancellationToken)
                .ConfigureAwait(false);
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
    }
}