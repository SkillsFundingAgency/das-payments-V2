using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SFA.DAS.Payments.Audit.Application.Data.DataLock;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.Audit.Application.Data.FundingSource;
using SFA.DAS.Payments.Audit.Application.Data.RequiredPayment;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public interface IAuditDataContext
    {
        DbSet<EarningEventModel> EarningEvent { get; }
        DbSet<EarningEventPeriodModel> EarningEventPeriod { get;  }
        DbSet<EarningEventPriceEpisodeModel> EarningEventPriceEpisode { get; }
        DbSet<RequiredPaymentEventModel> RequiredPayment { get; }
        DbSet<FundingSourceEventModel> FundingSourceEvent { get; }
        DbSet<DataLockEventModel> DataLockEvent { get; }
        DatabaseFacade Database { get; }
        Task<int> SaveChanges(CancellationToken cancellationToken = default(CancellationToken));
    }

    public class AuditDataContext : DbContext, IAuditDataContext
    {
        protected readonly string connectionString;

        public virtual DbSet<EarningEventModel> EarningEvent { get; protected set; }
        public virtual DbSet<EarningEventPeriodModel> EarningEventPeriod { get; protected set; }
        public virtual DbSet<EarningEventPriceEpisodeModel> EarningEventPriceEpisode { get; protected set; }
        public virtual DbSet<RequiredPaymentEventModel> RequiredPayment { get; protected set; }
        public virtual DbSet<FundingSourceEventModel> FundingSourceEvent { get; protected set; }
        public virtual DbSet<DataLockEventModel> DataLockEvent { get; protected set; }
        public virtual DbSet<DataLockEventPriceEpisodeModel> DataLockEventPriceEpisode { get; protected set; }
        public virtual DbSet<DataLockEventPayablePeriodModel> DataLockEventPayablePeriod { get; protected set; }
        public virtual DbSet<DataLockEventNonPayablePeriodModel> DataLockEventNonPayablePeriod { get; protected set; }
        public virtual DbSet<DataLockEventNonPayablePeriodFailureModel> DataLockEventNonPayablePeriodFailure { get; protected set; }

        public AuditDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public AuditDataContext(DbContextOptions<AuditDataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new EarningEventModelConfiguration());
            modelBuilder.ApplyConfiguration(new EarningEventPeriodModelConfiguration());
            modelBuilder.ApplyConfiguration(new EarningEventPriceEpisodeModelConfiguration());
            modelBuilder.ApplyConfiguration(new RequiredPaymentEventModelConfiguration());
            modelBuilder.ApplyConfiguration(new FundingSourceEventModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockEventModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockEventPriceEpisodeModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockEventPayablePeriodModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockEventNonPayablePeriodModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockEventNonPayablePeriodFailureModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (connectionString != null)
                optionsBuilder.UseSqlServer(connectionString);
        }

        public async Task<int> SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}