using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Repositories
{
    public class PaymentsDataContext : DbContext, IPaymentsDataContext
    {
        private readonly string connectionString;

        public PaymentsDataContext()
        {
        }

        public PaymentsDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public PaymentsDataContext(DbContextOptions options, string connectionString): base(options)
        {
            this.connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.Entity<PaymentDataEntity>().ToTable("Payment");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public virtual DbSet<PaymentDataEntity> Payment { get; set; }
    }
}
