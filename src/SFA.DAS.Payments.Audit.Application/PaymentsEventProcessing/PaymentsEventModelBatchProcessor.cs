using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Data;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing
{
    public interface IPaymentsEventModelBatchProcessor<T> : IBatchProcessor<T> where T : IPaymentsEventModel
    {
    }

    public class PaymentsEventModelBatchProcessor<T> : IPaymentsEventModelBatchProcessor<T> where T : IPaymentsEventModel
    {
        private readonly IPaymentsEventModelCache<T> cache;
        private readonly IPaymentsEventModelDataTable<T> dataTable;
        private readonly IPaymentLogger logger;
        private readonly string connectionString;

        public PaymentsEventModelBatchProcessor(
            IPaymentsEventModelCache<T> cache,
            IPaymentsEventModelDataTable<T> dataTable,
            IConfigurationHelper configurationHelper,
            IPaymentLogger logger)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.dataTable = dataTable ?? throw new ArgumentNullException(nameof(dataTable));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            connectionString = configurationHelper.GetConnectionString("PaymentsConnectionString") ?? throw new ArgumentException("Failed to find the PaymentsConnectionString in the ConnectionStrings section.");
        }

        protected virtual Task<bool> AllowPayment(T paymentModel)
        {
            return Task.FromResult(true);
        }

        public async Task<int> Process(int batchSize, CancellationToken cancellationToken)
        {
            logger.LogVerbose("Processing batch.");
            var batch = await cache.GetPayments(batchSize, cancellationToken);
            if (batch.Count < 1)
            {
                logger.LogVerbose("No records found to process.");
                return 0;
            }

            logger.LogDebug($"Processing {batch.Count} records: {string.Join(", ", batch.Select(m => m.EventId))}");

            var data = dataTable.GetDataTable(batch);
            using (var scope = TransactionScopeFactory.CreateWriteOnlyTransaction())
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync(cancellationToken);
                    using (var bulkCopy = new SqlBulkCopy(sqlConnection))
                    {
                        foreach (var table in data)
                        {
                            bulkCopy.ColumnMappings.Clear();
                            bulkCopy.DestinationTableName = data.Count > 1 ? table.TableName : dataTable.TableName;
                            bulkCopy.BulkCopyTimeout = 0;
                            bulkCopy.BatchSize = batchSize;

                            foreach (DataRow tableRow in table.Rows)
                            {
                                logger.LogVerbose($"Saving row to table: {bulkCopy.DestinationTableName}, Row: {ToLogString(tableRow)}");
                            }

                            foreach (DataColumn dataColumn in table.Columns)
                                bulkCopy.ColumnMappings.Add(dataColumn.ColumnName, dataColumn.ColumnName);

                            try
                            {
                                await bulkCopy.WriteToServerAsync(table, cancellationToken).ConfigureAwait(false);
                            }
                            catch (SystemException ex)
                            {
                                logger.LogWarning($"Error bulk writing to server. Processing single records. \n\n" +
                                                  $"{ex.Message}\n\n" +
                                                  $"{ex.StackTrace}");
                                await TrySingleRecord(bulkCopy, table, sqlConnection).ConfigureAwait(false);
                            }
                        }

                        logger.LogDebug($"Finished bulk copying {batch.Count} of {typeof(T).Name} records.");
                    }

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

        private async Task<int> TrySingleRecord(SqlBulkCopy bulkCopy, DataTable table, SqlConnection connection)
        {
            bulkCopy.BatchSize = 1;
            var errors = 0;

            using (var dataReader = table.CreateDataReader())
            {
                var values = new object[dataReader.FieldCount];
                var singleRecordTable = new DataTable();
                var dataSchema = dataReader.GetSchemaTable();
                foreach (DataRow row in dataSchema.Rows)
                {
                    singleRecordTable.Columns.Add(new DataColumn(row["ColumnName"].ToString(), (Type)row["DataType"]));
                }

                while (dataReader.Read())
                {
                    dataReader.GetValues(values);
                    singleRecordTable.Rows.Clear();
                    singleRecordTable.LoadDataRow(values, true);

                    try
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            await connection.OpenAsync().ConfigureAwait(false);
                        }
                        await bulkCopy.WriteToServerAsync(singleRecordTable).ConfigureAwait(false);
                    }
                    catch (SystemException ex)
                    {
                        logger.LogError($"Single record failure with {bulkCopy.DestinationTableName}:\n\n" +
                                        $" {ToLogString(singleRecordTable.Rows[0])}", ex);
                        errors++;
                    }
                }
            }

            return errors;
        }

        private static string ToLogString(DataRow tableRow)
        {
            var logStringBuilder = new StringBuilder();

            for (var i = 0; i < tableRow.ItemArray.Length; i++)
            {
                if(tableRow.Table.Columns[i].ColumnName == "LearnerUln" || tableRow.Table.Columns[i].ColumnName == "Ukprn")
                    continue;
                logStringBuilder.Append(tableRow.Table.Columns[i].ColumnName)
                    .Append(": ")
                    .Append(tableRow.ItemArray[i] ?? "null")
                    .AppendLine(", ");
            }

            return logStringBuilder.ToString();
        }
    }
}