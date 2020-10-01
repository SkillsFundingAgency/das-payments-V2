using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public interface IAuditDataContext
    {
        DbSet<EarningEventModel> EarningEvent { get; }
        DbSet<EarningEventPeriodModel> EarningEventPeriod { get;  }
        DbSet<EarningEventPriceEpisodeModel> EarningEventPriceEpisode { get; }

        DatabaseFacade Database { get; }
        Task<int> SaveChanges(CancellationToken cancellationToken = default(CancellationToken));
    }

    public class AuditDataContext : DbContext, IAuditDataContext
    {
        protected readonly string connectionString;

        public virtual DbSet<EarningEventModel> EarningEvent { get; protected set; }
        public virtual DbSet<EarningEventPeriodModel> EarningEventPeriod { get; protected set; }
        public virtual DbSet<EarningEventPriceEpisodeModel> EarningEventPriceEpisode { get; protected set; }

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