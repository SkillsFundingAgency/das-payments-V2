using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ProviderPayments.Application.Data.Configurations;

namespace SFA.DAS.Payments.ProviderPayments.Application.Data
{
    public interface IProviderPaymentsDataContext
    {
        DbSet<PaymentModel> Payment { get; }
        DbSet<PaymentModelWithRequiredPaymentId> PaymentsWithRequiredPayments { get; }
        DbSet<CollectionPeriodModel> CollectionPeriod { get; }
        DbSet<JobModel> Job { get; }


        DatabaseFacade Database { get; }
        Task<int> SaveChanges(CancellationToken cancellationToken = default(CancellationToken));
    }

    public class ProviderPaymentsDataContext : DbContext, IProviderPaymentsDataContext
    {
        protected readonly string connectionString;

        public virtual DbSet<PaymentModel> Payment { get; set; }
        public virtual DbSet<PaymentModelWithRequiredPaymentId> PaymentsWithRequiredPayments { get; set; }
        public virtual DbSet<CollectionPeriodModel> CollectionPeriod { get; set; }
        public virtual DbSet<JobModel> Job { get; set; }


        public ProviderPaymentsDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public ProviderPaymentsDataContext(DbContextOptions<ProviderPaymentsDataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new PaymentModelConfiguration());
            modelBuilder.ApplyConfiguration(new CollectionPeriodModelConfiguration());
            modelBuilder.ApplyConfiguration(new JobModelConfiguration());
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