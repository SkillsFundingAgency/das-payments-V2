using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class ApprenticeshipPauseModelConfiguration : IEntityTypeConfiguration<ApprenticeshipPauseModel>
    {
        public void Configure(EntityTypeBuilder<ApprenticeshipPauseModel> builder)
        {
            builder.ToTable("ApprenticeshipPause", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.ApprenticeshipId).HasColumnName(@"ApprenticeshipId").IsRequired();
            builder.Property(x => x.PauseDate).HasColumnName(@"PauseDate").IsRequired();
            builder.Property(x => x.ResumeDate).HasColumnName(@"ResumeDate");
        }
    }
}