using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.Payments.Monitoring.Jobs.Data;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests.JobsDataContextTests
{
    class InMemoryMetricsQueryDataContext : JobsDataContext
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
