using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Entities;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Scripts;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public interface IVerificationService
    {
        Task<PaymentsValues> GetPaymentsData(DateTime runStartDateTime, short academicYear, byte collectionPeriod,
            bool populateEarnings, IList<long> ukprnList);


        Task<DcValues> GetDcEarningsData(short academicYear, byte collectionPeriod, List<long> ukprnList);
    }

    public class VerificationService : IVerificationService
    {
        private const string PaymentsQuerySql = "PaymentsQuery.sql";
        private const string DcEarnings = "DcEarnings.sql";
        private const string UkprnListToken = "@ukprnlist";
        private readonly Configuration configuration;

        public VerificationService(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<PaymentsValues> GetPaymentsData(DateTime runStartDateTime, short academicYear,
            byte collectionPeriod, bool populateEarnings, IList<long> ukprnList)
        {
            var sql = ScriptHelpers.GetSqlScriptText(PaymentsQuerySql);
            sql = sql.Replace("@ukprnList", string.Join(",", ukprnList));

            var paymentsMetrics = new PaymentsValues();

            var results = new Dictionary<string, decimal?>(StringComparer.OrdinalIgnoreCase);
            using (var connection = GetPaymentsConnectionString())
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandTimeout = configuration.SqlCommandTimeout.Seconds;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@monthendStartTime", runStartDateTime);
                    cmd.Parameters.AddWithValue("@startDate", runStartDateTime);
                    cmd.Parameters.AddWithValue("@collectionPeriod", collectionPeriod);
                    connection.Open();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        PopulatePaymentsResults(results, reader);
                        await reader.NextResultAsync();
                        await reader.ReadAsync();
                        PopulatePaymentsResults(results, reader);
                        await reader.NextResultAsync();
                        await reader.ReadAsync();
                        PopulatePaymentsResults(results, reader);
                    }
                }
            }

            SetMetricsFromResults(results, paymentsMetrics);

            return paymentsMetrics;
        }


        public async Task<DcValues> GetDcEarningsData(short academicYear, byte collectionPeriod, List<long> ukprnList)
        {
            var sql = ScriptHelpers.GetSqlScriptText(DcEarnings);
            sql = sql.Replace(UkprnListToken, string.Join(",", ukprnList));

            var results = new DcValues();


            using (var connection = GetDataStoreConnectionString(academicYear))
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandTimeout = configuration.SqlCommandTimeout.Seconds;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@collectionPeriod", collectionPeriod);

                    connection.Open();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var newValues = new DcContractTypeTotals();

                            Func<SqlDataReader, int, decimal> getValueAsNullableDecimal = (dataReader, i) =>
                                !dataReader.IsDBNull(i) ? (decimal) dataReader.GetValue(i) : (decimal) 0;

                            newValues.ContractType = (int) reader[0];
                            newValues.TT1 = getValueAsNullableDecimal(reader, 1);
                            newValues.TT2 = getValueAsNullableDecimal(reader, 2);
                            newValues.TT3 = getValueAsNullableDecimal(reader, 3);
                            newValues.TT4 = getValueAsNullableDecimal(reader, 4);
                            newValues.TT5 = getValueAsNullableDecimal(reader, 5);
                            newValues.TT6 = getValueAsNullableDecimal(reader, 6);
                            newValues.TT7 = getValueAsNullableDecimal(reader, 7);
                            newValues.TT8 = getValueAsNullableDecimal(reader, 8);
                            newValues.TT9 = getValueAsNullableDecimal(reader, 9);
                            newValues.TT10 = getValueAsNullableDecimal(reader, 10);
                            newValues.TT11 = getValueAsNullableDecimal(reader, 11);
                            newValues.TT12 = getValueAsNullableDecimal(reader, 12);
                            newValues.TT13 = getValueAsNullableDecimal(reader, 13);
                            newValues.TT14 = getValueAsNullableDecimal(reader, 14);
                            newValues.TT15 = getValueAsNullableDecimal(reader, 15);
                            newValues.TT16 = getValueAsNullableDecimal(reader, 16);

                            results.DcContractTypeTotals.Add(newValues);
                        }
                    }
                }
            }

            return results;
        }

        private static void SetMetricsFromResults(Dictionary<string, decimal?> results, PaymentsValues paymentsMetrics)
        {
            if (results.Any())
            {
                paymentsMetrics.RequiredPaymentsThisMonth = results["Required Payments made this month"] ?? 0m;
                paymentsMetrics.PaymentsPriorToThisMonthYtd = results["Payments made before this month YTD"] ?? 0m;
                paymentsMetrics.ExpectedPaymentsAfterPeriodEnd =
                    results["Expected Payments YTD after running Period End"] ?? 0m;
                paymentsMetrics.TotalPaymentsThisMonth = results["Total payments this month"] ?? 0m;
                paymentsMetrics.TotalAct1Ytd = results["Total ACT 1 payments YTD"] ?? 0m;
                paymentsMetrics.TotalAct2Ytd = results["Total ACT 2 payments YTD"] ?? 0m;
                paymentsMetrics.TotalPaymentsYtd = results["Total payments YTD"] ?? 0m;
                paymentsMetrics.HeldBackCompletionThisMonth = results["Held Back Completion Payments this month"] ?? 0m;
                paymentsMetrics.DasEarnings = results["DAS Earnings"] ?? 0m;
                paymentsMetrics.DataLockedEarnings = results["Datalocked Earnings"] ?? 0m;
                paymentsMetrics.DataLockedPayments = results["Datalocked Payments"] ?? 0m;
                paymentsMetrics.AdjustedDataLocks = results["Adjusted Datalocks"] ?? 0m;
            }
        }

        private void PopulatePaymentsResults(Dictionary<string, decimal?> results, SqlDataReader reader)
        {
            var columnNames = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .ToList();
            for (var i = 0; i < columnNames.Count; i++)
                results.Add(columnNames[i], !reader.IsDBNull(i) ? (decimal?) reader.GetValue(i) : null);
        }


        private SqlConnection GetDataStoreConnectionString(short year)
        {
            switch (year)
            {
                case 1819:
                    return new SqlConnection(configuration.GetConnectionString("ILR1819DataStoreConnectionString"));
                case 1920:
                    return new SqlConnection(configuration.GetConnectionString("ILR1920DataStoreConnectionString"));
                default:
                    throw new ArgumentOutOfRangeException($"The given year {year} is not supported");
            }
        }


        private SqlConnection GetPaymentsConnectionString()
        {
            return new SqlConnection(configuration.PaymentsConnectionString);
        }
    }
}