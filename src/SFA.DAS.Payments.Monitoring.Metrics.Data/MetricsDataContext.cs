using System;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
    public interface IMetricsDataContext
    {

    }

    public class MetricsDataContext: DbContext, IMetricsDataContext
    {
        private readonly string connectionString;

        public MetricsDataContext(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}