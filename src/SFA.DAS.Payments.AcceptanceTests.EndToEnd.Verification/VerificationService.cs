using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification
{
    public class VerificationService
    {
        public VerificationService()
        {
        }

        public string GetVerificationDataCsv(short academicYear, byte collectionPeriod, bool populateEarnings,
            DateTime startDateTime, DateTime endDateTime)
        {
            var sql = Scripts.ScriptHelpers.GetSqlScriptText("VerificationQuery.sql");
            using (SqlConnection connection = new SqlConnection(ConfigurationManager
                .ConnectionStrings["PaymentsConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@academicYear", academicYear);
                    cmd.Parameters.AddWithValue("@collectionPeriod", collectionPeriod);
                    cmd.Parameters.AddWithValue("@populateEarnings", populateEarnings);
                    cmd.Parameters.AddWithValue("@StartTime", startDateTime);
                    cmd.Parameters.AddWithValue("@EndTime", endDateTime);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        return CreateCsvFromDataReader(reader);
                    }
                }
            }
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