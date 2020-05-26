using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Data.DataLock
{
    public class DataLockEventPayablePeriodModelConfiguration : IEntityTypeConfiguration<DataLockEventPayablePeriodModel>
    {
        public void Configure(EntityTypeBuilder<DataLockEventPayablePeriodModel> builder)
        {
            builder.ToTable("DataLockEventPayablePeriod", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.DataLockEventId).HasColumnName(@"DataLockEventId").IsRequired();
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier");
            builder.Property(x => x.TransactionType).HasColumnName(@"TransactionType").IsRequired();
            builder.Property(x => x.DeliveryPeriod).HasColumnName(@"DeliveryPeriod").IsRequired();
            builder.Property(x => x.Amount).HasColumnName(@"Amount").IsRequired();
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage");
            builder.Property(x => x.LearningStartDate).HasColumnName(@"LearningStartDate");

            builder.HasOne(pp => pp.DataLockEvent)
                .WithMany(dle => dle.PayablePeriods)
                .HasPrincipalKey(dle => dle.EventId)
                .HasForeignKey(pp => pp.DataLockEventId);
        }
    }
}