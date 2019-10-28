using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using FastMember;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.ProviderPayments.Model.V1;

namespace SFA.DAS.Payments.ProviderPayments.Application.Repositories
{
    public interface ILegacyPaymentsRepository
    {
        Task WritePaymentInformation(IEnumerable<LegacyPaymentModel> payments,
            IEnumerable<LegacyRequiredPaymentModel> requiredPayments,
            IEnumerable<LegacyEarningModel> earnings);

        Task WriteMonthEndTrigger(CollectionPeriod collectionPeriod);
    }

    public class LegacyPaymentsRepository : ILegacyPaymentsRepository
    {
        private readonly string connectionString;

        public LegacyPaymentsRepository(IConfigurationHelper configurationHelper)
        {
            connectionString = configurationHelper.GetConnectionString("DasPeriodEndConnectionString");
        }

        public async Task WritePaymentInformation(
            IEnumerable<LegacyPaymentModel> payments, 
            IEnumerable<LegacyRequiredPaymentModel> requiredPayments,
            IEnumerable<LegacyEarningModel> earnings)
        {
            using(var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadUncommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            using(var connection = new SqlConnection(connectionString))
            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                bulkCopy.BatchSize = 1000;
                bulkCopy.BulkCopyTimeout = 3600;

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

                bulkCopy.DestinationTableName = "[Payments].[Period]";
                bulkCopy.ColumnMappings.Clear();
                PopulateBulkCopy(bulkCopy, typeof(LegacyEarningModel));

                using (var reader = ObjectReader.Create(earnings))
                {
                    await bulkCopy.WriteToServerAsync(reader).ConfigureAwait(false);
                }

                scope.Complete();
            }
        }

        public async Task WriteMonthEndTrigger(CollectionPeriod collectionPeriod)
        {

            var trigger = CreateTrigger(collectionPeriod);
            var triggerList = new List<LegacyPeriodModel> {trigger};

            using (var connection = new SqlConnection(connectionString))
            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                bulkCopy.BatchSize = 1000;
                bulkCopy.BulkCopyTimeout = 30;

                bulkCopy.DestinationTableName = "[Payments].[Periods]";
                PopulateBulkCopy(bulkCopy, typeof(LegacyPeriodModel));

                using (var reader = ObjectReader.Create(triggerList))
                {
                    await bulkCopy.WriteToServerAsync(reader).ConfigureAwait(false);
                }
            }
        }

        public LegacyPeriodModel CreateTrigger(CollectionPeriod collectionPeriod)
        {
            var now = DateTime.Now;

            var trigger = new LegacyPeriodModel
            {
                AccountDataValidAt = now,
                CommitmentDataValidAt = now,
                CompletionDateTime = now,
                PeriodName = $"{collectionPeriod.AcademicYear}-R{collectionPeriod.Period:D2}",
                CalendarMonth = collectionPeriod.Period > 5 ? collectionPeriod.Period - 5 : collectionPeriod.Period + 7,
                CalendarYear = collectionPeriod.Period > 5 ? collectionPeriod.AcademicYear % 100 + 2000 : collectionPeriod.AcademicYear / 100 + 2000,
            };

            return trigger;
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
