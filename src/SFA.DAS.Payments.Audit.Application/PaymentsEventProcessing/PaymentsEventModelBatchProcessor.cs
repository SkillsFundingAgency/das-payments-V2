using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing
{
    public interface IPaymentsEventModelBatchProcessor<T> where T : PaymentsEventModel
    {
        Task<int> Process(int batchSize, CancellationToken cancellationToken);
    }

    public class PaymentsEventModelBatchProcessor<T>: IPaymentsEventModelBatchProcessor<T> where T: PaymentsEventModel
    {
        private readonly IPaymentsEventModelCache<T> cache;
        private readonly IPaymentsEventModelDataTable<T> dataTable;
        private readonly IPaymentLogger logger;
        private readonly string connectionString;

        public PaymentsEventModelBatchProcessor(IPaymentsEventModelCache<T> cache, 
            IPaymentsEventModelDataTable<T> dataTable, IConfigurationHelper configurationHelper,
            IPaymentLogger logger
            )
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.dataTable = dataTable ?? throw new ArgumentNullException(nameof(dataTable));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            connectionString = configurationHelper.GetConnectionString("PaymentsConnectionString") ?? throw new ArgumentException("Failed to find the PaymentsConnectionString in the ConnectionStrings section.");
        }

        public async Task<int> Process(int batchSize, CancellationToken cancellationToken)
        {
            logger.LogVerbose("Processing batch.");
            var batch = await cache.GetPayments(batchSize);
            if (batch.Count < 1)
            {
                logger.LogVerbose("No records found to process.");
                return 0;
            }
            logger.LogDebug($"Processing {batch.Count} records.");
            var data = dataTable.GetDataTable(batch);
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync(cancellationToken);
                    using (var bulkCopy = new SqlBulkCopy(sqlConnection))
                    {
                        bulkCopy.DestinationTableName = dataTable.TableName;
                        await bulkCopy.WriteToServerAsync(data, cancellationToken);
                        scope.Complete();
                        logger.LogDebug($"Finished bulk copying {batch.Count} records.");
                    }
                }
            }
            return batch.Count;
        }
    }
}