using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Configuration
{
    public class JobModelConfiguration:  IEntityTypeConfiguration<JobModel>
    {
        public void Configure(EntityTypeBuilder<JobModel> builder)
        {
            builder.ToTable("Job", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"JobId").IsRequired();
            builder.Property(x => x.StartTime).HasColumnName(@"StartTime").IsRequired();
            builder.Property(x => x.EndTime).HasColumnName(@"EndTime");
            builder.Property(x => x.Status).HasColumnName(@"Status");
            //builder.HasMany<ProviderEarningsJobModel>(x => x.ProviderEarnings).WithOne(x => x.Job);
            //builder.HasMany<JobStepModel>(x => x.JobEvents).WithOne(x => x.Job);
        }
    }
}