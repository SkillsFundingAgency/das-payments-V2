using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Repositories
{
    public class PaymentsDataContext : DbContext, IPaymentsDataContext
    {
        private readonly string connectionString;
        public DbSet<LevyAccountModel> LevyAccount { get; protected set; }
        public virtual DbSet<PaymentModel> Payment { get; set; }

        public PaymentsDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new PaymentModelConfiguration());
            modelBuilder.ApplyConfiguration(new LevyAccountModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
