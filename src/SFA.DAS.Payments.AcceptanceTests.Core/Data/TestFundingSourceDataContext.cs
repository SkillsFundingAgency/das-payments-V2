using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Data.Configurations;
using SFA.DAS.Payments.FundingSource.Application.Data;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class TestFundingSourceDataContext : FundingSourceDataContext
    {
        public TestFundingSourceDataContext(string connectionString) : base(connectionString)
        {
        }

        public TestFundingSourceDataContext(DbContextOptions<FundingSourceDataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString, options => options.CommandTimeout(600));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ProviderModelConfiguration());
        }

    }
}
