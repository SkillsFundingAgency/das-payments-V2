using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Audit.Application.ServiceFabric.PaymentsEventModelCache
{
    public class PaymentsEventModelCache<T>: IPaymentsEventModelCache<T> where T: PaymentsEventModel
    {
        private readonly IReliableStateManagerTransactionProvider transactionProvider;
        private readonly IReliableConcurrentQueue<T> queue;
        private readonly IPaymentLogger logger;
        
        public PaymentsEventModelCache(
            IReliableStateManagerProvider reliableStateManagerProvider, 
            IReliableStateManagerTransactionProvider transactionProvider, 
            IPaymentLogger logger)
        {
            this.transactionProvider = transactionProvider ?? throw new ArgumentNullException(nameof(transactionProvider));
            this.logger = logger;
            queue = reliableStateManagerProvider.Current.GetOrAddAsync<IReliableConcurrentQueue<T>>("PaymentsEventModelQueue").Result;
        }

        public async Task AddPayment(T paymentsEventModel)
        {
            logger.LogDebug($"Adding payment: {paymentsEventModel.EventId} Transaction: {transactionProvider.Current.TransactionId}");
            await queue.EnqueueAsync(transactionProvider.Current, paymentsEventModel, CancellationToken.None);
            logger.LogDebug($"Event {paymentsEventModel.EventId} of type {typeof(T)} added to cache. Transaction {transactionProvider.Current.TransactionId}.");
        }

        public async Task<List<T>> GetPayments(int batchSize)
        {
            var list = new List<T>();
            for (var i = 0; i < batchSize; i++)
            {
                var ret = await queue.TryDequeueAsync(transactionProvider.Current, default(CancellationToken));
                if (ret.HasValue)
                {
                    list.Add(ret.Value);
                    logger.LogDebug($"Event {ret.Value.EventId} of type {typeof(T)} removed from cache. Transaction {transactionProvider.Current.TransactionId}.");
                }
                else
                {
                    break; //no more items
                }
            }

            if (list.Any())
            {
                logger.LogDebug($"Removing payments: {string.Join(", ", list.Select(x => x.EventId))} Transaction: {transactionProvider.Current.TransactionId}");
            }
            return list;
        }
    }
}