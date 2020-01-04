using System;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.Metrics.Data.Configuration;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
    public interface IMetricsDataContext
    {
        DbSet<SubmissionSummaryModel> SubmissionSummaries { get; }
        DbSet<DataLockedEarningsModel> DataLockedEarnings { get;  }
        DbSet<EarningsModel> Earnings { get;  }
        DbSet<RequiredPaymentsModel> RequiredPayments { get;  }
    }

    public class MetricsDataContext: DbContext, IMetricsDataContext
    {
        private readonly string connectionString;

        public virtual DbSet<SubmissionSummaryModel> SubmissionSummaries { get; set; }
        public virtual DbSet<DataLockedEarningsModel> DataLockedEarnings { get; set; }
        public virtual DbSet<EarningsModel> Earnings { get; set; }
        public virtual DbSet<RequiredPaymentsModel> RequiredPayments { get; set; }
        
        public MetricsDataContext(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Metrics");
            modelBuilder.ApplyConfiguration(new SubmissionSummaryModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockedEarningsModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockedEarningsModelConfiguration());
            modelBuilder.ApplyConfiguration(new RequiredPaymentsModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}