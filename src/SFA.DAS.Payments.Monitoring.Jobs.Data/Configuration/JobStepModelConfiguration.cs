using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Configuration
{
    public class JobStepModelConfiguration : IEntityTypeConfiguration<JobStepModel>
    {
        public void Configure(EntityTypeBuilder<JobStepModel> builder)
        {
            builder.ToTable("JobEvent", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"JobEventId").IsRequired();
            builder.Property(x => x.JobId).HasColumnName(@"JobId").IsRequired();
            builder.Property(x => x.MessageId).HasColumnName(@"EventId");
            builder.Property(x => x.ParentMessageId).HasColumnName(@"ParentEventId");
            builder.Property(x => x.StartTime).HasColumnName(@"StartTime");
            builder.Property(x => x.EndTime).HasColumnName(@"EndTime");
            builder.Property(x => x.Status).HasColumnName(@"Status");
            builder.Property(x => x.MessageName).HasColumnName(@"MessageName");
            builder.HasOne(x => x.Job)
                .WithMany()
                .HasForeignKey(x => x.JobId);
        }
    }
}