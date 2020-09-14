using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data.UnitTests.MetricsQueryDataContextTests
{
    class InMemoryMetricsQueryDataContext : MetricsQueryDataContext
    {
        public InMemoryMetricsQueryDataContext() : base("")
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("test", new InMemoryDatabaseRoot());
        }
    }
}
