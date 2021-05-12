using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.Metrics.Data.Configuration;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function.AcceptanceTests.Data
{
    public class MetricsTestDataContext : DbContext
    {
        private readonly string connectionString;
        public virtual DbSet<CollectionPeriodToleranceModel> CollectionPeriodTolerances { get; set; }
        public virtual DbSet<SubmissionSummaryModel> SubmissionSummaries { get; set; }
        public virtual DbSet<SubmissionsSummaryModel> SubmissionsSummaries { get; set; }
        public virtual DbSet<PeriodEndSummaryModel> PeriodEndSummaries { get; set; }
        public virtual DbSet<ProviderPeriodEndSummaryModel> ProviderPeriodEndSummaries { get; set; }

        public MetricsTestDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new SubmissionSummaryModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockCountsModelConfiguration());
            modelBuilder.ApplyConfiguration(new EarningsModelConfiguration());
            modelBuilder.ApplyConfiguration(new RequiredPaymentsModelConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderPeriodEndSummaryModelConfiguration());
            modelBuilder.ApplyConfiguration(new PeriodEndSummaryModelConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderPaymentTransactionModelConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderPaymentFundingSourceModelConfiguration());
            modelBuilder.ApplyConfiguration(new SubmissionsSummaryModelConfiguration());
            modelBuilder.ApplyConfiguration(new CollectionPeriodToleranceModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}