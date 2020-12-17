using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data.Configuration
{
    public class DataLockCountsTotalsModelConfiguration : IEntityTypeConfiguration<DataLockCountsTotalsModel>
    {
        public void Configure(EntityTypeBuilder<DataLockCountsTotalsModel> builder)
        {
            builder.ToTable("SubmissionsSummaryDataLockCounts", "Metrics");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SubmissionsSummaryId).HasColumnName(@"SubmissionsSummaryId").IsRequired();

            builder.OwnsOne(x => x.Amounts, amounts =>
            {
                amounts.Property(x => x.DataLock1).HasColumnName("DataLock1");
                amounts.Property(x => x.DataLock2).HasColumnName("DataLock2");
                amounts.Property(x => x.DataLock3).HasColumnName("DataLock3");
                amounts.Property(x => x.DataLock4).HasColumnName("DataLock4");
                amounts.Property(x => x.DataLock5).HasColumnName("DataLock5");
                amounts.Property(x => x.DataLock6).HasColumnName("DataLock6");
                amounts.Property(x => x.DataLock7).HasColumnName("DataLock7");
                amounts.Property(x => x.DataLock8).HasColumnName("DataLock8");
                amounts.Property(x => x.DataLock9).HasColumnName("DataLock9");
                amounts.Property(x => x.DataLock10).HasColumnName("DataLock10");
                amounts.Property(x => x.DataLock11).HasColumnName("DataLock11");
                amounts.Property(x => x.DataLock12).HasColumnName("DataLock12"); 
                amounts.Ignore(x => x.Total);
            });

            builder.HasOne(x => x.SubmissionsSummary).WithOne(x => x.DataLockMetricsTotals);
        }
    }
}