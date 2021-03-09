using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.PeriodEnd.Model;

namespace SFA.DAS.Payments.PeriodEnd.Application.Data.Configurations
{
    class ProvidersRequiringReprocessingConfiguration : IEntityTypeConfiguration<ProviderRequiringReprocessingEntity>
    {
        public void Configure(EntityTypeBuilder<ProviderRequiringReprocessingEntity> builder)
        {
            builder.ToTable("ProviderRequiringReprocessing", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Ukprn).IsRequired();
        }
    }
}
