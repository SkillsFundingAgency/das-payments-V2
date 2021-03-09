using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.PeriodEnd.Application.Data.Configurations;
using SFA.DAS.Payments.PeriodEnd.Model;

namespace SFA.DAS.Payments.PeriodEnd.Application.Data
{
    public interface IPeriodEndDataContext
    {
        DbSet<ProviderRequiringReprocessingEntity> ProvidersRequiringReprocessing { get; set; }

        Task<int> SaveChanges(CancellationToken cancellationToken = default(CancellationToken));
    }

    class PeriodEndDataContext : DbContext, IPeriodEndDataContext
    {
        protected readonly string connectionString;

        public DbSet<ProviderRequiringReprocessingEntity> ProvidersRequiringReprocessing { get; set; }

        public PeriodEndDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public PeriodEndDataContext(DbContextOptions<PeriodEndDataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new ProvidersRequiringReprocessingConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (connectionString != null)
                optionsBuilder.UseSqlServer(connectionString);
        }

        public async Task<int> SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveChangesAsync(cancellationToken);
        }
    }
}
