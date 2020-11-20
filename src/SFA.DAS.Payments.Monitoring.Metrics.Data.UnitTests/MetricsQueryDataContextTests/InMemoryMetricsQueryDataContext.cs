using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data.UnitTests.MetricsQueryDataContextTests
{
    class InMemoryMetricsQueryDataContext : MetricsQueryDataContext
    {
        public InMemoryMetricsQueryDataContext() : base(new DbContextOptionsBuilder().UseInMemoryDatabase("test", new InMemoryDatabaseRoot()).Options)
        {
        }
    }
}
