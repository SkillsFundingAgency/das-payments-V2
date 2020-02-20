using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    public class JobStorageService : IJobStorageService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobModelRepository jobModelRepository;
        private readonly IInProgressMessageRepository inProgressMessageRepository;
        private readonly ICompletedMessageRepository completedMessageRepository;
        private readonly IJobStatusRepository jobStatusRepository;

        public JobStorageService(
            IPaymentLogger logger,
            IJobModelRepository jobModelRepository,
            IInProgressMessageRepository inProgressMessageRepository,
            ICompletedMessageRepository completedMessageRepository,
            IJobStatusRepository jobStatusRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobModelRepository = jobModelRepository;
            this.inProgressMessageRepository = inProgressMessageRepository;
            this.completedMessageRepository = completedMessageRepository;
            this.jobStatusRepository = jobStatusRepository;
        }

        public async Task<bool> StoreNewJob(JobModel job, CancellationToken cancellationToken)
        {
            if (!job.DcJobId.HasValue)
                throw new InvalidOperationException($"No dc job id specified for the job. Job type: {job.JobType:G}");

            var cachedJob = await jobModelRepository.GetJob(job.DcJobId.Value, cancellationToken);
            if (cachedJob != null)
            {
                logger.LogDebug($"Job has already been stored.");
                return false;
            }
            cancellationToken.ThrowIfCancellationRequested();

            await jobModelRepository.UpsertJob(job, cancellationToken);
            return true;
        }

        public async Task SaveJobStatus(long jobId, JobStatus jobStatus, DateTimeOffset endTime, CancellationToken cancellationToken)
        {
            var job = await jobModelRepository.GetJob(jobId, cancellationToken).ConfigureAwait(false);
            if (job == null)
                throw new InvalidOperationException($"Job not stored in the cache. Job: {jobId}");

            job.Status = jobStatus;
            job.EndTime = endTime;

            await jobModelRepository.UpsertJob(job, cancellationToken);
        }

        public async Task<List<long>> GetCurrentJobs(CancellationToken cancellationToken)
        {
            return await jobModelRepository.GetJobIdsByQuery(x => x.Status == JobStatus.InProgress, cancellationToken);
        }

        public async Task StoreDcJobStatus(long jobId, bool succeeded, CancellationToken cancellationToken)
        {
            var job = await jobModelRepository.GetJob(jobId, cancellationToken).ConfigureAwait(false);
            if (job == null)
                throw new InvalidOperationException($"Job not stored in the cache. Job: {jobId}");

            job.DcJobSucceeded = succeeded;
            job.DcJobEndTime = DateTimeOffset.UtcNow;

            if (job.Status == JobStatus.TimedOut)
            {
                job.Status = succeeded ? JobStatus.CompletedWithErrors : JobStatus.DcTasksFailed;
                job.EndTime = DateTimeOffset.Now;
            }

            await jobModelRepository.UpsertJob(job, cancellationToken);
        }

        public async Task<JobModel> GetJob(long jobId, CancellationToken cancellationToken)
        {
            return await jobModelRepository.GetJob(jobId, CancellationToken.None);
        }

        public async Task<List<InProgressMessage>> GetInProgressMessages(long jobId, CancellationToken cancellationToken)
        {
            return await inProgressMessageRepository.GetOrAddInProgressMessages(jobId, cancellationToken);
        }

        public async Task RemoveInProgressMessages(long jobId, List<Guid> messageIdentifiers, CancellationToken cancellationToken)
        {
            await inProgressMessageRepository.RemoveInProgressMessages(jobId, messageIdentifiers, cancellationToken);
        }

        public async Task StoreInProgressMessages(long jobId, List<InProgressMessage> inProgressMessages, CancellationToken cancellationToken)
        {
            await inProgressMessageRepository.StoreInProgressMessages(jobId, inProgressMessages, cancellationToken);
        }

        public async Task<List<CompletedMessage>> GetCompletedMessages(long jobId, CancellationToken cancellationToken)
        {
            return await completedMessageRepository.GetOrAddCompletedMessages(jobId, cancellationToken);
        }

        public async Task RemoveCompletedMessages(long jobId, List<Guid> completedMessages, CancellationToken cancellationToken)
        {
            await completedMessageRepository.RemoveCompletedMessages(jobId, completedMessages, cancellationToken);
        }

        public async Task StoreCompletedMessage(CompletedMessage completedMessage, CancellationToken cancellationToken)
        {
            await completedMessageRepository.StoreCompletedMessage(completedMessage, cancellationToken);
        }

        public async Task<(bool hasFailedMessages, DateTimeOffset? endTime)> GetJobStatus(long jobId, CancellationToken cancellationToken)
        {
            return await jobStatusRepository.GetJobStatus(jobId, cancellationToken);
        }

        public async Task StoreJobStatus(long jobId, bool hasFailedMessages, DateTimeOffset? endTime, CancellationToken cancellationToken)
        {
            await jobStatusRepository.StoreJobStatus(jobId, hasFailedMessages, endTime, cancellationToken);
        }

        public async Task SaveDataLocksCompletionTime(long jobId, DateTimeOffset endTime, CancellationToken cancellationToken)
        {
            await jobModelRepository.SaveDataLocksCompletionTime(jobId, endTime, cancellationToken);
        }
    }
}