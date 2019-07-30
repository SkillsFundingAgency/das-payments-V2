using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Repositories
{
    public class BulkDeleteAndWriter<TEntity> : BulkWriter<TEntity>, IBulkDeleteAndWriter<TEntity> where TEntity : class
    {
        private readonly IBulkDeleteAndCopyConfiguration<TEntity> bulkDeleteAndCopyConfig;

        public BulkDeleteAndWriter(IConfigurationHelper configurationHelper, IPaymentLogger logger, IBulkDeleteAndCopyConfiguration<TEntity> bulkDeleteAndCopyConfig) 
            : base(configurationHelper, logger, bulkDeleteAndCopyConfig )
        {
            this.bulkDeleteAndCopyConfig = bulkDeleteAndCopyConfig;
        }

        public async Task DeleteAndFlush(List<long> existingRecordIds, CancellationToken cancellationToken)
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

                var sqlCommand = sqlConnection.CreateCommand();

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.Transaction = transaction;

                        sqlCommand.CommandText = $@"delete from {bulkCopyConfig.TableName} Where {bulkDeleteAndCopyConfig.BulkDeleteFilterColumnName} in ({string.Join(",", existingRecordIds)})";
                        sqlCommand.ExecuteNonQuery();

                        using (var bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, transaction)) await HandleBulkCopy(cancellationToken, list, bulkCopy);

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        logger.LogError(
                            $"Error while trying to bulk Add or Update {list.Count} records of type {typeof(TEntity).Name}",
                            e);
                        throw;
                    }
                }

                logger.LogDebug($"Saved {list.Count} records of type {typeof(TEntity).Name}");
            }
        }


    }
}