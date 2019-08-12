using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using FastMember;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ProviderPayments.Model.V1;

namespace SFA.DAS.Payments.ProviderPayments.Application.Repositories
{
    public interface ILegacyPaymentsRepository
    {
        Task WritePaymentInformation(IEnumerable<LegacyPaymentModel> payments,
            IEnumerable<LegacyRequiredPaymentModel> requiredPayments);
    }

    public class LegacyPaymentsRepository
    {
        private readonly string connectionString;

        public LegacyPaymentsRepository(IConfigurationHelper configurationHelper)
        {
            connectionString = configurationHelper.GetConnectionString("DasPeriodEndConnectionString");
        }

        public async Task WritePaymentInformation(IEnumerable<LegacyPaymentModel> payments, IEnumerable<LegacyRequiredPaymentModel> requiredPayments)
        {
            using(var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadUncommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            using(var connection = new SqlConnection(connectionString))
            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                bulkCopy.DestinationTableName = "[PaymentsDue].[RequiredPayments]";
                PopulateBulkCopy(bulkCopy, typeof(LegacyRequiredPaymentModel));

                using (var reader = ObjectReader.Create(requiredPayments))
                {
                    await bulkCopy.WriteToServerAsync(reader).ConfigureAwait(false);
                }

                bulkCopy.DestinationTableName = "[Payments].[Payments]";
                bulkCopy.ColumnMappings.Clear();
                PopulateBulkCopy(bulkCopy, typeof(LegacyPaymentModel));

                using (var reader = ObjectReader.Create(payments))
                {
                    await bulkCopy.WriteToServerAsync(reader).ConfigureAwait(false);
                }

                scope.Complete();
            }
        }

        private void PopulateBulkCopy(SqlBulkCopy bulkCopy, Type entityType)
        {
            var columns = entityType.GetProperties();
            foreach (var propertyInfo in columns)
            {
                bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(propertyInfo.Name, propertyInfo.Name));
            }
        }
    }
}
