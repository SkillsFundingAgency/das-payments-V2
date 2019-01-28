using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Audit.AcceptanceTests.Data.Configuration;
using SFA.DAS.Payments.Audit.AcceptanceTests.Data.Entities;

namespace SFA.DAS.Payments.Audit.AcceptanceTests.Data
{
    public class AuditDataContext: DbContext
    {
        private readonly string connectionString;
        public virtual DbSet<FundingSourceEvent> FundingSourceEvents { get; set; }

        public AuditDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new FundingSourceEventConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}