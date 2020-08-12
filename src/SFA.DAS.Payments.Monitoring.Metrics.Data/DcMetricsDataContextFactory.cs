using System.Collections.Concurrent;
using System.Data.SqlClient;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
    public interface IDcMetricsDataContextFactory
    {
        IDcMetricsDataContext CreateContext(short academicYear);
    }

    public class DcMetricsDataContextFactory : IDcMetricsDataContextFactory
    {
        private readonly string connectionString;
        private readonly ConcurrentDictionary<short, IDcMetricsDataContext> contexts;

        public DcMetricsDataContextFactory(string connectionString)
        {
            this.connectionString = connectionString;
            contexts = new ConcurrentDictionary<short, IDcMetricsDataContext>();
        }

        public IDcMetricsDataContext CreateContext(short academicYear)
        {
            if (contexts.ContainsKey(academicYear))
                return contexts[academicYear];

            var builder = new SqlConnectionStringBuilder(connectionString);
            builder["Database"] = $"ILR{academicYear}DataStore";
            contexts.AddOrUpdate(academicYear, new DcMetricsDataContext(builder.ConnectionString), (ay, c) => c);
            return contexts[academicYear];
        }
    }
}