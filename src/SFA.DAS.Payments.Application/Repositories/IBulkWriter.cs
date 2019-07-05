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
        Task Write(TEntity entity, CancellationToken cancellationToken = default);
        Task Write(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task Flush(CancellationToken cancellationToken = default);
    }

    public class BulkWriter<TEntity> : IBulkWriter<TEntity> where TEntity : class
    {
        private readonly int batchSize;
        private readonly ConcurrentQueue<TEntity> queue = new ConcurrentQueue<TEntity>();
        private readonly string connectionString;
        private readonly IPaymentLogger logger;
        private readonly IBulkCopyConfiguration<TEntity> bulkCopyConfig;

        public BulkWriter(IConfigurationHelper configurationHelper, IPaymentLogger logger, IBulkCopyConfiguration<TEntity> bulkCopyConfig)
        {
            this.logger = logger;
            this.bulkCopyConfig = bulkCopyConfig;
            batchSize = configurationHelper.GetSettingOrDefault("batchSize", 500);
            connectionString = configurationHelper.GetConnectionString("PaymentsConnectionString");
        }

        public async Task Write(TEntity entity, CancellationToken cancellationToken = default)
        {
            queue.Enqueue(entity);

            if (queue.Count < batchSize)
                return;

            await Flush(cancellationToken);
        }

        public async Task Write(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task>();
            foreach (var entity in entities)
            {
                tasks.Add(Write(entity, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        public async Task Flush(CancellationToken cancellationToken = default)
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
}
