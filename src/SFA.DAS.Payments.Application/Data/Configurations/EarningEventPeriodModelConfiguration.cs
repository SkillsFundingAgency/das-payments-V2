using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class EarningEventPeriodModelConfiguration : IEntityTypeConfiguration<EarningEventPeriodModel>
    {
        public void Configure(EntityTypeBuilder<EarningEventPeriodModel> builder)
        {
            builder.ToTable("EarningEventPeriod", "Payments2");
            builder.Property(x => x.EarningEventId).HasColumnName(@"EarningEventId").IsRequired();
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier");
            builder.Property(x => x.TransactionType).HasColumnName(@"TransactionType").IsRequired();
            builder.Property(x => x.DeliveryPeriod).HasColumnName(@"DeliveryPeriod").IsRequired();
            builder.Property(x => x.Amount).HasColumnName(@"Amount").IsRequired();
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage");
        }
    }
}
