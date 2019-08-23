using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Configuration
{
    public class JobMessageFinishedModelConfiguration : IEntityTypeConfiguration<JobMessageFinishedModel>
    {
        public void Configure(EntityTypeBuilder<JobMessageFinishedModel> builder)
        {
            builder.ToTable("JobMessageFinished", "Jobs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"JobMessageStartedId").IsRequired();
            builder.Property(x => x.JobId).HasColumnName(@"JobId").IsRequired();
            builder.Property(x => x.MessageId).HasColumnName(@"MessageId");
            builder.Property(x => x.EndTime).HasColumnName(@"EndTime");
            builder.Property(x => x.Status).HasColumnName(@"Status");
            builder.Property(x => x.MessageName).HasColumnName(@"MessageName");
            builder.HasOne(x => x.Job)
                .WithMany()
                .HasForeignKey(x => x.JobId);
        }
    }
}