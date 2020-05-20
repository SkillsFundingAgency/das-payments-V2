using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data.Entities
{
    public class EarningEventPeriod
    {
        public long Id { get; set; }
        public Guid EarningEventId { get; set; }
        public int TransactionType { get; set; }
        public int DeliveryPeriod { get; set; }
        public decimal Amount { get; set; }
    }

    public class EarningEventPeriodConfiguration : IEntityTypeConfiguration<EarningEventPeriod>
    {
        public void Configure(EntityTypeBuilder<EarningEventPeriod> builder)
        {
            builder.ToTable("EarningEventPeriod", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.EarningEventId).HasColumnName(@"EarningEventId").IsRequired();
            builder.Property(x => x.TransactionType).HasColumnName(@"TransactionType");
            builder.Property(x => x.DeliveryPeriod).HasColumnName(@"DeliveryPeriod");
            builder.Property(x => x.Amount).HasColumnName(@"Amount");
        }
    }
}