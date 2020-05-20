using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data.Entities
{
    public class DataLockPayablePeriod
    {
        public long Id { get; set; }
        public Guid DataLockEventId { get; set; }
        public byte TransactionType { get; set; }
        public byte DeliveryPeriod { get; set; }
        public decimal Amount { get; set; }
    }

    public class DataLockPayablePeriodConfiguration : IEntityTypeConfiguration<DataLockPayablePeriod>
    {
        public void Configure(EntityTypeBuilder<DataLockPayablePeriod> builder)
        {
            builder.ToTable("DataLockEventPayablePeriod", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.DataLockEventId).HasColumnName(@"DataLockEventId").IsRequired();
            builder.Property(x => x.TransactionType).HasColumnName(@"TransactionType").IsRequired();
            builder.Property(x => x.DeliveryPeriod).HasColumnName(@"DeliveryPeriod");
            builder.Property(x => x.Amount).HasColumnName(@"Amount");
        }
    }
}