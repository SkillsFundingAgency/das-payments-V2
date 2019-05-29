using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class ApprenticeshipDuplicateModelConfiguration : IEntityTypeConfiguration<ApprenticeshipDuplicateModel>
    {
        public void Configure(EntityTypeBuilder<ApprenticeshipDuplicateModel> builder)
        {
            builder.ToTable("ApprenticeshipDuplicate", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.OwnsOne(p => p.Apprenticeship, a =>
            {
                a.Property(x => x.Id).HasColumnName(@"ApprenticeshipId").IsRequired();
            });
            builder.Property(x => x.Uln).HasColumnName(@"Uln").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn").IsRequired();
        }
    }
}
