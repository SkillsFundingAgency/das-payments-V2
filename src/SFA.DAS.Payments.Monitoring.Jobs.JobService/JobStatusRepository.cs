using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    public class JobStatusRepository : IJobStatusRepository
    {
        private readonly IReliableStateManagerProvider stateManagerProvider;
        private readonly IReliableStateManagerTransactionProvider reliableTransactionProvider;
        private static readonly TimeSpan TransactionTimeout = new TimeSpan(0, 0, 4);

        public const string JobStatusCacheKey = "job_status";

        public JobStatusRepository(IReliableStateManagerProvider stateManagerProvider,
            IReliableStateManagerTransactionProvider reliableTransactionProvider)
        {
            this.stateManagerProvider = stateManagerProvider ?? throw new ArgumentNullException(nameof(stateManagerProvider));
            this.reliableTransactionProvider = reliableTransactionProvider ?? throw new ArgumentNullException(nameof(reliableTransactionProvider));
        }

        private async Task<IReliableDictionary2<long, (bool hasFailedMessages, DateTimeOffset? endTime)>> GetJobStatusCollection(long jobId)
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