using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public interface IVerificationService
    {
        Task<string> GetPaymentsDataCsv(DateTime runStartDateTime, short academicYear, byte collectionPeriod,
            bool populateEarnings, IList<long> ukprnList);
        Task<(decimal? missingPayments, decimal? earningsYtd)?> GetPaymentTotals(short academicYear, byte collectionPeriod, bool populateEarnings, IList<long> ukprnList);

        Task<string> GetEarningsCsv(short academicYear, byte collectionPeriod, List<long> ukprnList);

        Task<DateTimeOffset?> GetLastActivityDate(List<long> ukprns);

        Task<decimal?> GetTotalEarningsYtd(short academicYear, byte collectionPeriod, List<long> ukprnList);
    }

    public class VerificationService : IVerificationService
    {
        private const string PaymentsQuerySql = "PaymentsQuery.sql";
        private const string EarningsCteSql = "EarningsCte.sql";
        private const string EarningsDetailSql = "EarningsDetail.sql";
        private const string EarningsSummarySql = "EarningsSummary.sql";
        private const string UkprnListToken = "@ukprnlist";
        private readonly Configuration configuration;

        public VerificationService(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> GetPaymentsDataCsv(DateTime runStartDateTime, short academicYear,
            byte collectionPeriod, bool populateEarnings, IList<long> ukprnList)
        {
            var sql = Scripts.ScriptHelpers.GetSqlScriptText(PaymentsQuerySql);
            sql = sql.Replace("@ukprnList", String.Join(",", ukprnList));
            using (SqlConnection connection = GetPaymentsConnectionString())
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandTimeout = configuration.SqlCommandTimeout.Seconds;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@monthendStartTime", runStartDateTime);
                    cmd.Parameters.AddWithValue("@startDate", runStartDateTime);
                    cmd.Parameters.AddWithValue("@collectionPeriod", collectionPeriod);
                    connection.Open();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {

                        Dictionary<string, decimal?> results = new Dictionary<string, decimal?>();
                        await reader.ReadAsync();

                        PoplateResults(results, reader);
                        await reader.NextResultAsync();
                        await reader.ReadAsync();
                        PoplateResults(results, reader);
                        await reader.NextResultAsync();
                        await reader.ReadAsync();
                        PoplateResults(results, reader);
                    }
                }
            }

            return "";

        }

        private void PoplateResults(Dictionary<string, decimal?> results, SqlDataReader reader)
        {
           
            var columnNames = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .ToList();
            for (int i = 0; i < columnNames.Count; i++)
            {
                results.Add(columnNames[i],!reader.IsDBNull(i) ? (decimal?) reader.GetValue(i) : (decimal?) null);
            }
        }
    

        public async Task<string> GetEarningsCsv(short academicYear, byte collectionPeriod, List<long> ukprnList)
        {
            var sql = Scripts.ScriptHelpers.GetSqlScriptText(EarningsCteSql);
            sql += Scripts.ScriptHelpers.GetSqlScriptText(EarningsDetailSql);
            sql = sql.Replace(UkprnListToken, string.Join(",", ukprnList));

            using (SqlConnection connection = GetDataStoreConnectionString(academicYear))
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandTimeout = configuration.SqlCommandTimeout.Seconds;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@collectionPeriod", collectionPeriod);
                    
                    connection.Open();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        return CreateCsvFromDataReader(reader);
                    }
                }
            }
        }


        public async Task<DateTimeOffset?> GetLastActivityDate(List<long> ukprns)
        {
            var ukprnsString = String.Join(",", ukprns);

            String sql = $@"select max(CreationDate) from (
						select ukprn, CreationDate from Payments2.EarningEvent
						union all
						select ukprn, CreationDate from Payments2.DataLockEvent
						union all
						select ukprn, CreationDate from Payments2.RequiredPaymentEvent
						union all
						select ukprn, CreationDate from Payments2.FundingSourceEvent
					) as d
					where ukprn in ({ukprnsString})";


            using (SqlConnection connection = GetPaymentsConnectionString())
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandTimeout = configuration.SqlCommandTimeout.Seconds;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    connection.Open();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.IsDBNull(result) ? (DateTimeOffset?)null : (DateTimeOffset?)result;
                }

            }
        }

        public async Task<decimal?> GetTotalEarningsYtd(short academicYear, byte collectionPeriod, List<long> ukprnList)
        {
            var sql = Scripts.ScriptHelpers.GetSqlScriptText(EarningsCteSql);
            sql += Scripts.ScriptHelpers.GetSqlScriptText(EarningsSummarySql);
            sql = sql.Replace(UkprnListToken, string.Join(",", ukprnList));

            using (SqlConnection connection = GetDataStoreConnectionString(academicYear))
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandTimeout = configuration.SqlCommandTimeout.Seconds;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@collectionPeriod", collectionPeriod);

                    connection.Open();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.IsDBNull(result) ? (decimal?) null : (decimal?) result;
                }
            }
        }

        public async Task<(decimal? missingPayments, decimal? earningsYtd)?> GetPaymentTotals(short academicYear, byte collectionPeriod, bool populateEarnings,IList<long> ukprnList)
        {
            var sql = @"select sum([Missing Required Payments]) As MissingRequiredPayments, sum([Earnings YTD (audit)]) As EarningsYtd
            FROM(
            ";

            sql +=   Scripts.ScriptHelpers.GetSqlScriptText(PaymentsQuerySql);
            sql += @"
                ) as sq
            where[Transaction Type] in (1, 2, 3)";

            using (SqlConnection connection = GetPaymentsConnectionString())
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandTimeout = configuration.SqlCommandTimeout.Seconds;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@academicYear", academicYear);
                    cmd.Parameters.AddWithValue("@collectionPeriod", collectionPeriod);
                    cmd.Parameters.AddWithValue("@populateEarnings", populateEarnings);
                    connection.Open();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var missingPayments =
                                (!reader.IsDBNull(0)) ? (decimal?) reader.GetValue(0) : (decimal?) null;
                            var earningsYtd =
                                (!reader.IsDBNull(1)) ? (decimal?) reader.GetValue(1) : (decimal?) null;
                            return (missingPayments, earningsYtd);
                        }
                    }

                    return null;
                }
            }
        }



        private SqlConnection GetDataStoreConnectionString(short year)
        {
            switch (year)
            {
                case 1819:
                 return   new SqlConnection(configuration.GetConnectionString("ILR1819DataStoreConnectionString"));
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


        private static string CreateCsvFromDataReader(SqlDataReader reader)
        {
            StringBuilder sb = new StringBuilder();

            var columnNames = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .ToList();
            sb.Append(string.Join(",", columnNames));
            sb.AppendLine();

            while (reader.Read())
            {
                var rowData = Enumerable.Range(0, reader.FieldCount)
                    .Select(i => (!reader.IsDBNull(i)) ? reader.GetValue(i).ToString() : (string) null)
                    .ToList();
                sb.Append(string.Join(",", rowData));
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}