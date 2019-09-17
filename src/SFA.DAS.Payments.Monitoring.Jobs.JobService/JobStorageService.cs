using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    public class JobStorageService : IJobStorageService
    {
        private readonly IReliableStateManagerProvider stateManagerProvider;
        private readonly IReliableStateManagerTransactionProvider reliableTransactionProvider;
        private readonly IJobsDataContext dataContext;
        private readonly IPaymentLogger logger;
        private static readonly TimeSpan TransactionTimeout = new TimeSpan(0, 0, 4);

        public const string JobCacheKey = "jobs";
        public const string JobStatusCacheKey = "job_status";
        public const string InProgressMessagesCacheKey = "inprogress_messages";
        public const string CompletedMessagesCacheKey = "completed_messages";

        public JobStorageService(IReliableStateManagerProvider stateManagerProvider,
            IReliableStateManagerTransactionProvider reliableTransactionProvider,
            IJobsDataContext dataContext, IPaymentLogger logger)
        {
            this.stateManagerProvider = stateManagerProvider ?? throw new ArgumentNullException(nameof(stateManagerProvider));
            this.reliableTransactionProvider = reliableTransactionProvider ?? throw new ArgumentNullException(nameof(reliableTransactionProvider));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        private static string GetCacheKey(string cacheKeyPrefix, long jobId) => $"{cacheKeyPrefix}_{jobId}";

        private async Task<IReliableDictionary2<long,JobModel>> GetJobCollection()
        {
            return await stateManagerProvider.Current.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobCacheKey);
        }

        public async Task<bool> StoreNewJob(JobModel job, CancellationToken cancellationToken)
        {
            if (!job.DcJobId.HasValue)
                throw new InvalidOperationException($"No dc job id specified for the job. Job type: {job.JobType:G}");
            cancellationToken.ThrowIfCancellationRequested();
            var jobCache = await GetJobCollection();
            var cachedJob = await jobCache.TryGetValueAsync(reliableTransactionProvider.Current, job.DcJobId.Value, TransactionTimeout, cancellationToken).ConfigureAwait(false);
            if (cachedJob.HasValue)
            {
                logger.LogDebug($"Job has already been stored.");
                return false;
            }

            await jobCache.AddOrUpdateAsync(reliableTransactionProvider.Current, job.DcJobId.Value,
                id => job, (id,existingJob) => job, TransactionTimeout, cancellationToken)
                .ConfigureAwait(false);

            if (job.Id == 0)
                await dataContext.SaveNewJob(job, cancellationToken).ConfigureAwait(false);
            return true;
        }

        public async Task SaveJobStatus(long jobId, JobStatus jobStatus, DateTimeOffset endTime,  CancellationToken cancellationToken)
        {
            await dataContext.SaveJobStatus(jobId, jobStatus, endTime, cancellationToken).ConfigureAwait(false);
        }

        public async Task<JobModel> GetJob(long jobId, CancellationToken cancellationToken)
        {
            var collection = await GetJobCollection().ConfigureAwait(false);
            var item = await collection
                .TryGetValueAsync(reliableTransactionProvider.Current, jobId, TransactionTimeout, cancellationToken)
                .ConfigureAwait(false);
            return item.Value;
        }

        private async Task<IReliableDictionary2<Guid, object>> GetInProgressMessagesCollection(long jobId)
        {
            return await stateManagerProvider.Current.GetOrAddAsync<IReliableDictionary2<Guid, object>>(
                $"{InProgressMessagesCacheKey}_{jobId}");
        }

        public async Task<List<Guid>> GetInProgressMessageIdentifiers(long jobId, CancellationToken cancellationToken)
        {
            var inProgressCollection = await GetInProgressMessagesCollection(jobId).ConfigureAwait(false);
            var enumerator = (await inProgressCollection.CreateEnumerableAsync(reliableTransactionProvider.Current)).GetAsyncEnumerator();
            var identifiers = new List<Guid>();

            while (await enumerator.MoveNextAsync(cancellationToken))
            {
                identifiers.Add(enumerator.Current.Key);
            }
            return identifiers;
        }


        public async Task RemoveInProgressMessageIdentifiers(long jobId, List<Guid> messageIdentifiers, CancellationToken cancellationToken)
        {
            var inProgressCollection = await GetInProgressMessagesCollection(jobId).ConfigureAwait(false);
            foreach (var messageIdentifier in messageIdentifiers)
            {
                await inProgressCollection.TryRemoveAsync(reliableTransactionProvider.Current, messageIdentifier,
                        TransactionTimeout, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public async Task StoreInProgressMessageIdentifiers(long jobId, List<Guid> inProgressMessageIdentifiers, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cachedInprogressItems = await GetInProgressMessagesCollection(jobId);
            foreach (var messageIdentifier in inProgressMessageIdentifiers)
            {
                await cachedInprogressItems.AddOrUpdateAsync(reliableTransactionProvider.Current, messageIdentifier,
                        key => messageIdentifier, (key, value) => messageIdentifier, TransactionTimeout,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private async Task<IReliableDictionary2<Guid, CompletedMessage>> GetCompletedMessagesCollection(long jobId)
        {
            return await stateManagerProvider.Current.GetOrAddAsync<IReliableDictionary2<Guid, CompletedMessage>>(
                $"{CompletedMessagesCacheKey}_{jobId}").ConfigureAwait(false);
        }

        public async Task<List<CompletedMessage>> GetCompletedMessages(long jobId, CancellationToken cancellationToken)
        {
            var completedMessageCollection = await GetCompletedMessagesCollection(jobId).ConfigureAwait(false);
            var enumerator = (await completedMessageCollection.CreateEnumerableAsync(reliableTransactionProvider.Current)).GetAsyncEnumerator();
            var identifiers = new List<CompletedMessage>();

            while (await enumerator.MoveNextAsync(cancellationToken))
            {
                identifiers.Add(enumerator.Current.Value);
            }
            return identifiers;
        }

        public async Task RemoveCompletedMessages(long jobId, List<Guid> completedMessages, CancellationToken cancellationToken)
        {
            var completedMessagesCollection = await GetCompletedMessagesCollection(jobId).ConfigureAwait(false);
            foreach (var completedMessage in completedMessages)
            {
                await completedMessagesCollection.TryRemoveAsync(reliableTransactionProvider.Current, completedMessage,
                    TransactionTimeout, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task StoreCompletedMessage(CompletedMessage completedMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var completedMessagesCollection =
                await GetCompletedMessagesCollection(completedMessage.JobId).ConfigureAwait(false);
            await completedMessagesCollection.AddOrUpdateAsync(reliableTransactionProvider.Current,
                    completedMessage.MessageId,
                    completedMessage, (key, value) => completedMessage, TransactionTimeout, cancellationToken)
                .ConfigureAwait(false);
        }


        private async Task<IReliableDictionary2<long,(bool hasFailedMessages, DateTimeOffset? endTime)>> GetJobStatusCollection(long jobId)
        {
            return await stateManagerProvider.Current.GetOrAddAsync<IReliableDictionary2<long, (bool hasFailedMessages, DateTimeOffset? endTime)>>(JobStatusCacheKey).ConfigureAwait(false);
        }

        public async Task<(bool hasFailedMessages, DateTimeOffset? endTime)> GetJobStatus(long jobId, CancellationToken cancellationToken)
        {
            var collection = await GetJobStatusCollection(jobId).ConfigureAwait(false);
            var value = await collection
                .TryGetValueAsync(reliableTransactionProvider.Current, jobId, TransactionTimeout, cancellationToken)
                .ConfigureAwait(false);
            return value.HasValue ? value.Value : (hasFailedMessages: false, endTime: null);
        }

        public async Task StoreJobStatus(long jobId, bool hasFailedMessages, DateTimeOffset? endTime, CancellationToken cancellationToken)
        {
            var collection = await GetJobStatusCollection(jobId).ConfigureAwait(false);
            await collection.AddOrUpdateAsync(reliableTransactionProvider.Current, jobId,
                (hasFailedMessages: hasFailedMessages, endTime: endTime),
                (key, value) => (hasFailedMessages: hasFailedMessages, endTime: endTime),
                TransactionTimeout, cancellationToken).ConfigureAwait(false);
        }
    }
}