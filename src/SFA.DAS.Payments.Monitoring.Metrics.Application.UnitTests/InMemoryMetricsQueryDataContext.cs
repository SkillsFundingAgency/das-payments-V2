using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests
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
