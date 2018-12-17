using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Model;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Configuration
{
    public class ProviderEarningsJobModelConfiguration : IEntityTypeConfiguration<ProviderEarningsJobModel>
    {
        public void Configure(EntityTypeBuilder<ProviderEarningsJobModel> builder)
        {
            builder.ToTable("ProviderEarningsJob", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"JobId").IsRequired();
            builder.Property(x => x.DcJobId).HasColumnName(@"DCJobId").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn");
            builder.Property(x => x.IlrSubmissionTime).HasColumnName(@"IlrSubmissionTime");
            builder.Property(x => x.CollectionYear).HasColumnName(@"CollectionYear");
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod");
            builder.HasOne<JobModel>(x => x.Job).WithOne();
        }
    }
}