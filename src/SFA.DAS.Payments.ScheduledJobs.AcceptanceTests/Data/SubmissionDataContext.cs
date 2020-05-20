using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data.Entities;

namespace SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data
{
    public class SubmissionDataContext: DbContext
    {
        private readonly string connectionString;
        public virtual DbSet<EarningEvent> EarningEvents { get; set; }
        public virtual DbSet<EarningEventPeriod> EarningEventPeriods { get; set; }
        public virtual DbSet<EarningEventPriceEpisode> EarningEventPriceEpisodes { get; set; }
        public virtual DbSet<FundingSourceEvent> FundingSourceEvents { get; set; }
        public virtual DbSet<RequiredPaymentEvent> RequiredPaymentEvents { get; set; }
        public virtual DbSet<DataLockEvent> DataLockEvents { get; set; }
        public virtual DbSet<DataLockPayablePeriod> DataLockPayablePeriods { get; set; }
        public virtual DbSet<DataLockEventNonPayablePeriod> DataLockEventNonPayablePeriods { get; set; }
        public virtual DbSet<DataLockEventNonPayablePeriodFailures> DataLockEventNonPayablePeriodFailures { get; set; }
        public virtual DbSet<DataLockEventPriceEpisode> DataLockEventPriceEpisodes { get; set; }
        public virtual DbSet<JobModel> Jobs { get; set; }

        public SubmissionDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new EarningEventConfiguration());
            modelBuilder.ApplyConfiguration(new EarningEventPeriodConfiguration());
            modelBuilder.ApplyConfiguration(new EarningEventPriceEpisodeConfiguration());

            modelBuilder.ApplyConfiguration(new FundingSourceEventConfiguration());

            modelBuilder.ApplyConfiguration(new RequiredPaymentEventConfiguration());

            modelBuilder.ApplyConfiguration(new DataLockEventConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockPayablePeriodConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockEventNonPayablePeriodConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockEventNonPayablePeriodFailuresConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockEventPriceEpisodeConfiguration());

            modelBuilder.ApplyConfiguration(new JobModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}