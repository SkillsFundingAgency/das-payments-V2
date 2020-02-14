using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    public class JobModelRepository : IJobModelRepository
    {
        private readonly IReliableStateManagerProvider stateManagerProvider;
        private readonly IReliableStateManagerTransactionProvider reliableTransactionProvider;
        private readonly IJobsDataContext dataContext;
        private static readonly TimeSpan TransactionTimeout = new TimeSpan(0, 0, 4);

        public const string JobCacheKey = "jobs";

        public JobModelRepository(IReliableStateManagerProvider stateManagerProvider,
            IReliableStateManagerTransactionProvider reliableTransactionProvider,
            IJobsDataContext dataContext)
        {
            this.stateManagerProvider = stateManagerProvider ?? throw new ArgumentNullException(nameof(stateManagerProvider));
            this.reliableTransactionProvider = reliableTransactionProvider ?? throw new ArgumentNullException(nameof(reliableTransactionProvider));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        private async Task<IReliableDictionary2<long, JobModel>> GetJobCollection()
        {
            return await stateManagerProvider.Current.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobCacheKey);
        }

        public async Task UpsertJob(JobModel job, CancellationToken cancellationToken)
        {
            if (!job.DcJobId.HasValue)
                throw new InvalidOperationException($"No dc job id specified for the job. Job type: {job.JobType:G}");
            cancellationToken.ThrowIfCancellationRequested();

            var jobCache = await GetJobCollection();
            await jobCache.AddOrUpdateAsync(reliableTransactionProvider.Current, job.DcJobId.Value,
                    id => job, (id, existingJob) => job, TransactionTimeout, cancellationToken)
                .ConfigureAwait(false);

            await dataContext.UpdateJob(job, cancellationToken);
        }

        public async Task<JobModel> GetJob(long jobId, CancellationToken cancellationToken)
        {
            var collection = await GetJobCollection().ConfigureAwait(false);
            var item = await collection
                .TryGetValueAsync(reliableTransactionProvider.Current, jobId, TransactionTimeout, cancellationToken)
                .ConfigureAwait(false);
            return item.Value;
        }

        public async Task<List<long>> GetJobIdsByQuery(Func<JobModel, bool> query, CancellationToken cancellationToken)
        {
            var collection = await GetJobCollection().ConfigureAwait(false);
            var jobIds = new List<long>();
            var enumerator = (await collection.CreateEnumerableAsync(reliableTransactionProvider.Current)).GetAsyncEnumerator();
            while (await enumerator.MoveNextAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var job = enumerator.Current.Value;
                if(query(job) && job.DcJobId.HasValue) jobIds.Add(job.DcJobId.Value);
            }
            return jobIds;
        }

        public async Task SaveDataLocksCompletionTime(long jobId, DateTimeOffset endTime, CancellationToken cancellationToken)
        {
            await dataContext.SaveDataLocksCompletionTime(jobId, endTime, cancellationToken).ConfigureAwait(
                false);
        }
    }
}