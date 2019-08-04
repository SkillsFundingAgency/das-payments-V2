using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Application.Batch
{
    public interface IBatchProcessor<T>
    {
        Task<int> Process(int batchSize, CancellationToken cancellationToken);
    }

    public class BatchProcessor<T> : IBatchProcessor<T> where T : class
    {
        private readonly IBatchedDataCache<T> cache;
        private readonly IPaymentLogger logger;
        private readonly IBulkWriter<T> bulkWriter;

        public BatchProcessor(IBatchedDataCache<T> cache, IPaymentLogger logger, IBulkWriter<T> bulkWriter)
        {
            this.cache = cache;
            this.logger = logger;
            this.bulkWriter = bulkWriter;
        }

        public async Task<int> Process(int batchSize, CancellationToken cancellationToken)
        {
            logger.LogVerbose("Processing batch.");
            var batch = await cache.GetPayments(batchSize, cancellationToken).ConfigureAwait(false);
            if (batch.Count < 1)
            {
                logger.LogVerbose("No records found to process.");
                return 0;
            }

            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var item in batch)
                    {
                        logger.LogVerbose($"Saving {typeof(T).Name} to table: {item.ToString()}");
                        await bulkWriter.Write(item, cancellationToken).ConfigureAwait(false);
                    }

                    await bulkWriter.Flush(cancellationToken).ConfigureAwait(false);
                    scope.Complete();
                }
                catch (Exception e)
                {
                    logger.LogError($"Error performing bulk copy for model type: {typeof(T).Name}. Error: {e.Message}", e);
                    throw;
                }
            }

            return batch.Count;

        }
    }
}