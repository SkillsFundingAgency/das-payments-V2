﻿using System.Collections.Immutable;
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
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id");
            builder.Property(x => x.EarningEventId).HasColumnName(@"EarningEventId");
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier");
            builder.Property(x => x.TransactionType).HasColumnName(@"TransactionType");
            builder.Property(x => x.DeliveryPeriod).HasColumnName(@"DeliveryPeriod");
            builder.Property(x => x.Amount).HasColumnName(@"Amount");
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage");
            builder.Property(x => x.CensusDate).HasColumnName(@"CensusDate");
            builder.HasOne(x => x.EarningEvent)
                .WithMany(ee => ee.Periods)
                .HasPrincipalKey(x => x.EventId)
                .HasForeignKey(x => x.EarningEventId);
        }
    }
}
