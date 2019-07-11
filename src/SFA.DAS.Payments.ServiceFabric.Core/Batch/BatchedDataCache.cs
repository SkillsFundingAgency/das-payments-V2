using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.ServiceFabric.Core.Batch
{
    public class BatchedDataCache<T> : IBatchedDataCache<T>
    {
        private readonly IReliableStateManagerTransactionProvider transactionProvider;
        private readonly IReliableConcurrentQueue<T> queue;
        private readonly IPaymentLogger logger;

        public BatchedDataCache(IReliableStateManagerTransactionProvider transactionProvider, IReliableStateManagerProvider reliableStateManagerProvider, IPaymentLogger logger)
        {
            this.transactionProvider = transactionProvider ?? throw new ArgumentNullException(nameof(transactionProvider));
            this.logger = logger;
            if (reliableStateManagerProvider == null) throw new ArgumentNullException(nameof(reliableStateManagerProvider));
            queue = reliableStateManagerProvider.Current.GetOrAddAsync<IReliableConcurrentQueue<T>>("BatchedDataCacheQueue").Result;
        }

        public async Task AddPayment(T model, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Adding object: {Describe(model)} Transaction: {transactionProvider.Current.TransactionId}");
            await queue.EnqueueAsync(transactionProvider.Current, model, cancellationToken);
            logger.LogDebug($"Object {Describe(model)} of type {typeof(T)} added to cache. Transaction {transactionProvider.Current.TransactionId}.");
        }

        public async Task<List<T>> GetPayments(int batchSize, CancellationToken cancellationToken)
        {
            var list = new List<T>();
            for (var i = 0; i < batchSize; i++)
            {
                var ret = await queue.TryDequeueAsync(transactionProvider.Current, cancellationToken);
                if (ret.HasValue)
                {
                    list.Add(ret.Value);
                    logger.LogDebug($"Object {Describe(ret.Value)} of type {typeof(T)} removed from cache. Transaction {transactionProvider.Current.TransactionId}.");
                }
                else
                {
                    break; //no more items
                }
            }

            if (list.Any())
            {
                logger.LogDebug($"Removing object: {string.Join(", ", list.Select(Describe))} Transaction: {transactionProvider.Current.TransactionId}");
            }
            return list;
        }

        protected virtual string Describe(T message)
        {
            return message.ToString();
        }
    }
}