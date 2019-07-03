using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data.Configurations
{
    public class ProviderModelConfiguration : IEntityTypeConfiguration<TestModels.Provider>
    {
        public void Configure(EntityTypeBuilder<TestModels.Provider> builder)
        {
            builder.ToTable("TestingProvider", "Payments2");
            builder.HasKey(x => x.Ukprn);
            builder.HasIndex(x => x.LastUsed);
            builder.Property(x => x.LastUsed).HasColumnName("LastUsed");
            builder.Property(x => x.Ukprn).HasColumnName("Ukprn");
        }
    }
}
