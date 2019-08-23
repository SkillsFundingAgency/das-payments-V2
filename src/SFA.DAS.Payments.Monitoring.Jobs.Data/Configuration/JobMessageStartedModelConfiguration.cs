using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Configuration
{
    public class JobMessageStartedModelConfiguration : IEntityTypeConfiguration<JobMessageStartedModel>
    {
        public void Configure(EntityTypeBuilder<JobMessageStartedModel> builder)
        {
            builder.ToTable("JobMessageStarted", "Jobs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"JobMessageStartedId").IsRequired();
            builder.Property(x => x.JobId).HasColumnName(@"JobId").IsRequired();
            builder.Property(x => x.MessageId).HasColumnName(@"MessageId");
            builder.Property(x => x.ParentMessageId).HasColumnName(@"ParentMessageId");
            builder.Property(x => x.StartTime).HasColumnName(@"StartTime");
            builder.Property(x => x.MessageName).HasColumnName(@"MessageName");
            builder.HasOne(x => x.Job)
                .WithMany()
                .HasForeignKey(x => x.JobId);
        }
    }
}