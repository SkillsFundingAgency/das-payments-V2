using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobStorageService
    {
        Task StoreNewJob(JobModel job, CancellationToken cancellationToken);
        Task UpdateJob(JobModel job, CancellationToken cancellationToken);
        Task<JobModel> GetJob(CancellationToken cancellationToken);
        Task<List<JobStepModel>> GetJobMessages(List<Guid> messageIds, CancellationToken cancellationToken);
        Task<bool> StoredJobMessage(Guid messageId, CancellationToken cancellationToken);
        Task StoreJobMessages(List<JobStepModel> jobMessages, CancellationToken cancellationToken);
        Task<List<Guid>> GetInProgressMessageIdentifiers(CancellationToken cancellationToken);
        Task StoreInProgressMessageIdentifiers(List<Guid> inProgressMessageIdentifiers,
            CancellationToken cancellationToken);

        Task<(JobStepStatus jobStatus, DateTimeOffset? endTime)> GetJobStatus(CancellationToken cancellationToken);
        Task StoreJobStatus(JobStepStatus jobStatus, DateTimeOffset? endTime, CancellationToken cancellationToken);
    }

    public class JobStorageService : IJobStorageService
    {
        private readonly IActorDataCache<JobModel> jobCache;
        private readonly IActorDataCache<JobStepModel> jobMessagesCache;
        private readonly IActorDataCache<List<Guid>> inProgressMessageCache;
        private readonly IActorDataCache<(JobStepStatus jobStatus, DateTimeOffset? endTime)> jobStatusCache;
        private readonly IJobsDataContext dataContext;
        private readonly IPaymentLogger logger;
        public const string JobCacheKey = "job";
        public const string JobStatusCacheKey = "job_status";
        public const string InProgressMessagesCacheKey = "in_progress_messages";

        public JobStorageService(IActorDataCache<JobModel> jobCache,
            IActorDataCache<JobStepModel> jobMessagesCache,
            IActorDataCache<List<Guid>> inProgressMessageCache,
            IActorDataCache<(JobStepStatus jobStatus, DateTimeOffset? endTime)> jobStatusCache,
            IJobsDataContext dataContext, IPaymentLogger logger)
        {
            this.jobCache = jobCache ?? throw new ArgumentNullException(nameof(jobCache));
            this.jobMessagesCache = jobMessagesCache ?? throw new ArgumentNullException(nameof(jobMessagesCache));
            this.inProgressMessageCache = inProgressMessageCache ?? throw new ArgumentNullException(nameof(inProgressMessageCache));
            this.jobStatusCache = jobStatusCache ?? throw new ArgumentNullException(nameof(jobStatusCache));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StoreNewJob(JobModel job, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cachedJob = await jobCache.TryGet(JobCacheKey, cancellationToken).ConfigureAwait(false);
            if (cachedJob.HasValue)
            {
                logger.LogDebug($"Job has already been stored.");
                return;
            }

            await jobCache.AddOrReplace(JobCacheKey, job, cancellationToken).ConfigureAwait(false);
            if (job.Id == 0)
                await dataContext.SaveNewJob(job, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateJob(JobModel job, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await jobCache.AddOrReplace(JobCacheKey, job, cancellationToken).ConfigureAwait(false);
            await dataContext.SaveNewJob(job, cancellationToken).ConfigureAwait(false);
        }

        public async Task<JobModel> GetJob(CancellationToken cancellationToken)
        {
            var cacheItem = await jobCache.TryGet(JobCacheKey, cancellationToken).ConfigureAwait(false);
            return cacheItem.HasValue ? cacheItem.Value : null;
        }

        public async Task<List<JobStepModel>> GetJobMessages(List<Guid> messageIds, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var jobMessages = new List<JobStepModel>();
            //TODO: must be a more performant way of doing this
            foreach (var messageId in messageIds)
            {
                var cachedJobMessage = await jobMessagesCache.TryGet(messageId.ToString(), cancellationToken).ConfigureAwait(false);
                if (cachedJobMessage.HasValue)
                    jobMessages.Add(cachedJobMessage.Value);
            }

            return jobMessages;
        }

        public async Task<bool> StoredJobMessage(Guid messageId, CancellationToken cancellationToken)
        {
            return await jobMessagesCache.Contains(messageId.ToString(), cancellationToken).ConfigureAwait(false);
        }

        public async Task StoreJobMessages(List<JobStepModel> jobMessages, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var jobMessage in jobMessages)
            {
                await jobMessagesCache.AddOrReplace(jobMessage.MessageId.ToString(), jobMessage, cancellationToken);
            }
        }

        public async Task<List<Guid>> GetInProgressMessageIdentifiers(CancellationToken cancellationToken)
        {
            var cacheItem = await inProgressMessageCache.TryGet(InProgressMessagesCacheKey, cancellationToken)
                .ConfigureAwait(false);
            return cacheItem.HasValue ? cacheItem.Value : new List<Guid>();
        }

        public async Task StoreInProgressMessageIdentifiers(List<Guid> inProgressMessageIdentifiers, CancellationToken cancellationToken)
        {
            await inProgressMessageCache
                .AddOrReplace(InProgressMessagesCacheKey, inProgressMessageIdentifiers, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<(JobStepStatus jobStatus, DateTimeOffset? endTime)> GetJobStatus(CancellationToken cancellationToken)
        {
            var cacheItem = await jobStatusCache.TryGet(JobStatusCacheKey, cancellationToken).ConfigureAwait(false);
            return cacheItem.HasValue ? cacheItem.Value : (JobStepStatus.Processing, null);
        }

        public async Task StoreJobStatus(JobStepStatus jobStatus, DateTimeOffset? endTime, CancellationToken cancellationToken)
        {
            await jobStatusCache.AddOrReplace(JobStatusCacheKey, (jobStatus, endTime), cancellationToken).ConfigureAwait(false);
        }
    }
}