using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastMember;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Batch
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
            connectionString = bulkCopyConfig.ConnectionString;
        }

        public async Task Write(TEntity entity, CancellationToken cancellationToken)
        {
            queue.Enqueue(entity);

            if (queue.Count < batchSize)
                return;

            await Flush(cancellationToken);
        }

        public virtual async Task Flush(CancellationToken cancellationToken)
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

                try
                {
                    await bulkCopy.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false);
                }
                catch (SystemException)
                {
                    logger.LogWarning("Error bulk writing to server. Processing single records.");
                    await TrySingleRecord(bulkCopy, list, cancellationToken);
                }
            }

            logger.LogDebug($"Saved {list.Count} records of type {typeof(TEntity).Name}");
        }

        private async Task<int> TrySingleRecord(SqlBulkCopy bulkCopy, List<TEntity> list, CancellationToken cancellationToken)
        {
            var errors = 0;

            foreach (var entity in list)
            {
                try
                {
                    using (var reader = ObjectReader.Create(new List<TEntity> {entity}))
                    {
                        await bulkCopy.WriteToServerAsync(reader, cancellationToken);
                    }
                }
                catch (SystemException ex)
                {
                    logger.LogError($"Single record failure: {LogProperties(entity)}", ex);
                    errors++;
                }
            }

            return errors;
        }

        private string LogProperties(TEntity entity)
        {
            var builder = new StringBuilder($"Entity type: {typeof(TEntity).Name}");
            foreach (var propertyInfo in typeof(TEntity).GetProperties())
            {
                builder.AppendLine($"{propertyInfo.Name}: {propertyInfo.GetValue(entity)}");
            }

            return builder.ToString();
        }
    }
}
