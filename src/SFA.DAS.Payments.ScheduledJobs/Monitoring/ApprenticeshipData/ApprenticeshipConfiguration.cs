using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData
{
    public class ApprenticeshipConfiguration : IEntityTypeConfiguration<ApprenticeshipModel>
    {
        public void Configure(EntityTypeBuilder<ApprenticeshipModel> builder)
        {
            builder.Property(e => e.Cost).HasColumnType("decimal(18, 0)");
            builder.Property(e => e.CreatedOn).HasColumnType("datetime");
            builder.Property(e => e.DateOfBirth).HasColumnType("datetime");
            builder.Property(e => e.EmployerRef).HasMaxLength(50);
            builder.Property(e => e.EndDate).HasColumnType("datetime");

            builder.Property(e => e.EpaOrgId)
                .HasColumnName("EPAOrgId")
                .HasMaxLength(7)
                .IsUnicode(false);

            builder.Property(e => e.FirstName).HasMaxLength(100);
            builder.Property(e => e.LastName).HasMaxLength(100);

            builder.Property(e => e.NiNumber)
                .HasColumnName("NINumber")
                .HasMaxLength(10);


            builder.Property(e => e.ProviderRef).HasMaxLength(50);
            builder.Property(e => e.StartDate).HasColumnType("datetime");


            builder.Property(e => e.ProgrammeType)
                .HasColumnName("TrainingType");

            builder.Property(e => e.CourseCode)
                .HasColumnName("TrainingCode")
                .HasMaxLength(20);

            builder.Property(e => e.ProgrammeType).HasColumnName("TrainingType");

            builder.Property(e => e.CourseName)
                .HasColumnName("TrainingName")
                .HasMaxLength(126);

            builder.Property(e => e.Uln)
                .HasColumnName("Uln")
                .HasMaxLength(50);

            builder.Property(e => e.ProgrammeType).HasColumnName("TrainingType");

            builder.Ignore(e => e.IsProviderSearch);
            builder.HasOne(a => a.Commitment)
                .WithMany()
                .HasForeignKey(c => c.CommitmentId);
        }
    }
}
