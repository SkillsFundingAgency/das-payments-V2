using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests
{
    class InMemoryMetricsQueryDataContext : MetricsQueryDataContext
    {
        public InMemoryMetricsQueryDataContext() : base(new DbContextOptionsBuilder().UseInMemoryDatabase("test", new InMemoryDatabaseRoot()).Options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("test", new InMemoryDatabaseRoot())
                .ConfigureWarnings(builder => builder.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        }
    }
}
