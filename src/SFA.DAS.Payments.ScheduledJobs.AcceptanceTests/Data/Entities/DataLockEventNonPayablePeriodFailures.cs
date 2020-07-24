using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data.Entities
{
    public class DataLockEventNonPayablePeriodFailures
    {
        public long Id { get; set; }
        public Guid DataLockEventNonPayablePeriodId { get; set; }
        public byte DataLockFailureId { get; set; }
    }

    public class DataLockEventNonPayablePeriodFailuresConfiguration : IEntityTypeConfiguration<DataLockEventNonPayablePeriodFailures>
    {
        public void Configure(EntityTypeBuilder<DataLockEventNonPayablePeriodFailures> builder)
        {
            builder.ToTable("DataLockEventNonPayablePeriodFailures", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.DataLockFailureId).HasColumnName(@"DataLockFailureId").IsRequired();
            builder.Property(x => x.DataLockEventNonPayablePeriodId).HasColumnName(@"DataLockEventNonPayablePeriodId").IsRequired();
        }
    }
}