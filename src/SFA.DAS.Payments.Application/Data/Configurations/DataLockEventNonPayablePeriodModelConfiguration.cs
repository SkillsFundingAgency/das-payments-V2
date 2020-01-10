using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class DataLockEventNonPayablePeriodModelConfiguration : IEntityTypeConfiguration<DataLockEventNonPayablePeriodModel>
    {
        public void Configure(EntityTypeBuilder<DataLockEventNonPayablePeriodModel> builder)
        {
            builder.ToTable("DataLockEventNonPayablePeriod", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.DataLockEventId).HasColumnName(@"DataLockEventId").IsRequired();
            builder.Property(x => x.DataLockEventNonPayablePeriodId).HasColumnName(@"DataLockEventNonPayablePeriodId").IsRequired();
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier");
            builder.Property(x => x.TransactionType).HasColumnName(@"TransactionType").IsRequired();
            builder.Property(x => x.DeliveryPeriod).HasColumnName(@"DeliveryPeriod").IsRequired();
            builder.Property(x => x.Amount).HasColumnName(@"Amount").IsRequired();
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage");
            builder.Property(x => x.CensusDate).HasColumnName(@"CensusDate");
            builder.Property(x => x.LearningStartDate).HasColumnName(@"LearningStartDate");

            builder.HasOne(x => x.DataLockEvent).WithMany(dl => dl.NonPayablePeriods).HasForeignKey(x => x.DataLockEventId);
            //builder.HasMany(x => x.Failures).WithOne(y => y.DataLockEventNonPayablePeriod).HasPrincipalKey(y => y.DataLockEventNonPayablePeriodId);
        }
    }
}