using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class CollectionPeriodModelConfiguration : IEntityTypeConfiguration<CollectionPeriodModel>
    {
        public void Configure(EntityTypeBuilder<CollectionPeriodModel> builder)
        {
            builder.ToTable("CollectionPeriod", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear").IsRequired();
            builder.Property(x => x.Period).HasColumnName(@"Period").IsRequired();
            builder.Property(x => x.ReferenceDataValidationDate).HasColumnName(@"ReferenceDataValidationDate");
            builder.Property(x => x.CompletionDate).HasColumnName(@"CompletionDate").IsRequired();
        }
    }
}