using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class PeriodEndEventModelConfiguration : IEntityTypeConfiguration<PeriodEndEventModel>
    {
        public void Configure(EntityTypeBuilder<PeriodEndEventModel> builder)
        {
            builder.ToTable("PeriodEndEvent", "Payments2");
            builder.HasKey(x => x.EventId);
            builder.Property(x => x.EventId).HasColumnName(@"EventId").IsRequired();
            builder.Property(x => x.JobId).HasColumnName(@"JobId").IsRequired();
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear").IsRequired();
            builder.Property(x => x.Period).HasColumnName(@"Period").IsRequired();
            builder.Property(x => x.EventTime).HasColumnName(@"EventTime");
        }
    }
}