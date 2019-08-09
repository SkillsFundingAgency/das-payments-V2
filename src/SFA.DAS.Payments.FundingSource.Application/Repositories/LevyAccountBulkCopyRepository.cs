using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Repositories
{

    public interface ILevyAccountBulkCopyRepository : IBulkWriter<LevyAccountModel>
    {
        Task DeleteAndFlush(List<long> existingRecordIds, CancellationToken cancellationToken);
    }

    public class LevyAccountBulkCopyRepository : BulkWriter<LevyAccountModel>, ILevyAccountBulkCopyRepository
    {
        public LevyAccountBulkCopyRepository(IConfigurationHelper configurationHelper,
            IPaymentLogger logger,
            IBulkCopyConfiguration<LevyAccountModel> bulkCopyConfig) : base(configurationHelper, logger, bulkCopyConfig)
        {
        }

        public override async Task Write(LevyAccountModel entity, CancellationToken cancellationToken)
        {
            queue.Enqueue(entity);
            await Task.CompletedTask;
        }


        public async Task DeleteAndFlush(List<long> existingRecordIds, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Saving {queue.Count} records to LevyAccount Table");

            var list = new List<LevyAccountModel>();
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
                        sqlCommand.CommandText = SplitDeleteQueryToBatch(existingRecordIds);

                      await sqlCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                        using (var bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, transaction))
                        {
                            await HandleBulkCopy(cancellationToken, list, bulkCopy).ConfigureAwait(false);
                        }

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        logger.LogError($"Error while trying to bulk Add or Update {list.Count} records of type LevyAccountModel", e);

                        throw;
                    }
                }

                logger.LogDebug($"Saved {list.Count} records of type LevyAccountModel");
            }
        }

        private string SplitDeleteQueryToBatch(List<long> existingRecordIds)
        {
            var queryBuilder = new StringBuilder();
            const int queryParametersBatchSize = 1000;

            for (var i = 0; i < existingRecordIds.Count; i += queryParametersBatchSize)
            {
                var batchAccountIds = existingRecordIds.Skip(i).Take(queryParametersBatchSize).ToList();
                queryBuilder.AppendLine($"delete from [Payments2].[LevyAccount] Where AccountId in ({string.Join(",", batchAccountIds)}) ;");
            }

            return queryBuilder.ToString();
        }




    }
}