using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using FastMember;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Repositories
{
    public interface IBulkWriter<TEntity> where TEntity : class
    {
        Task Write(TEntity entity, CancellationToken cancellationToken);
        Task Flush(CancellationToken cancellationToken);
    }
    
    public class BulkWriter<TEntity> : IBulkWriter<TEntity> where TEntity : class
    {
        private readonly int batchSize;
        protected readonly ConcurrentQueue<TEntity> queue = new ConcurrentQueue<TEntity>();
        protected readonly string connectionString;
        protected readonly IPaymentLogger logger;
        protected readonly IBulkCopyConfiguration<TEntity> bulkCopyConfig;

        public BulkWriter(IConfigurationHelper configurationHelper, IPaymentLogger logger,
            IBulkCopyConfiguration<TEntity> bulkCopyConfig)
        {
            this.logger = logger;
            this.bulkCopyConfig = bulkCopyConfig;
            batchSize = configurationHelper.GetSettingOrDefault("batchSize", 500);
            connectionString = configurationHelper.GetConnectionString("PaymentsConnectionString");
        }

        public async Task Write(TEntity entity, CancellationToken cancellationToken)
        {
            queue.Enqueue(entity);

            if (queue.Count < batchSize)
                return;

            await Flush(cancellationToken);
        }

        public async Task Flush(CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Saving {queue.Count} records of type {typeof(TEntity).Name}");

            var list = new List<TEntity>();
            while (queue.TryDequeue(out var item))
            {
                list.Add(item);
            }

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

                using (var bulkCopy = new SqlBulkCopy(sqlConnection))
                {
                    await HandleBulkCopy(cancellationToken, list, bulkCopy).ConfigureAwait(false);
                }
            }
        }

        protected async Task HandleBulkCopy(CancellationToken cancellationToken, List<TEntity> list, SqlBulkCopy bulkCopy)
        {
            using (var reader = ObjectReader.Create(list))
            {
                foreach (var columnMap in bulkCopyConfig.Columns)
                {
                    bulkCopy.ColumnMappings.Add(columnMap.Key, columnMap.Value);
                }

                bulkCopy.BulkCopyTimeout = 0;
                bulkCopy.BatchSize = batchSize;
                bulkCopy.DestinationTableName = bulkCopyConfig.TableName;

                await bulkCopy.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false);
            }

            logger.LogDebug($"Saved {list.Count} records of type {typeof(TEntity).Name}");
        }
        
    }
}
