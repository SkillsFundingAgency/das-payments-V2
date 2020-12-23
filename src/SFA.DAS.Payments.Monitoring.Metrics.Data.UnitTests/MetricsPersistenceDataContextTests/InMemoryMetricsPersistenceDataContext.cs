using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data.UnitTests.MetricsPersistenceDataContextTests
{
    public class InMemoryMetricsPersistenceDataContext : MetricsPersistenceDataContext
    {
        public InMemoryMetricsPersistenceDataContext() : base(new DbContextOptionsBuilder().UseInMemoryDatabase("test", new InMemoryDatabaseRoot()).Options)
        {
        }
    }
}
