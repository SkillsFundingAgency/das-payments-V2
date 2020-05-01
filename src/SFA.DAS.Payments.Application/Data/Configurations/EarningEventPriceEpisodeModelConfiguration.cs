using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class EarningEventPriceEpisodeModelConfiguration : IEntityTypeConfiguration<EarningEventPriceEpisodeModel>
    {
        public void Configure(EntityTypeBuilder<EarningEventPriceEpisodeModel> builder)
        {
            builder.ToTable("EarningEventPriceEpisode", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.EarningEventId).HasColumnName(@"EarningEventId").IsRequired();
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier");
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage");
            builder.Property(x => x.TotalNegotiatedPrice1).HasColumnName(@"TotalNegotiatedPrice1");
            builder.Property(x => x.TotalNegotiatedPrice2).HasColumnName(@"TotalNegotiatedPrice2");
            builder.Property(x => x.TotalNegotiatedPrice3).HasColumnName(@"TotalNegotiatedPrice3");
            builder.Property(x => x.TotalNegotiatedPrice4).HasColumnName(@"TotalNegotiatedPrice4");
            builder.Property(x => x.StartDate).HasColumnName(@"StartDate");
            builder.Property(x => x.EffectiveTotalNegotiatedPriceStartDate).HasColumnName(@"EffectiveTotalNegotiatedPriceStartDate");
            builder.Property(x => x.PlannedEndDate).HasColumnName(@"PlannedEndDate");
            builder.Property(x => x.ActualEndDate).HasColumnName(@"ActualEndDate");
            builder.Property(x => x.NumberOfInstalments).HasColumnName(@"NumberOfInstalments");
            builder.Property(x => x.InstalmentAmount).HasColumnName(@"InstalmentAmount");
            builder.Property(x => x.CompletionAmount).HasColumnName(@"CompletionAmount");
            builder.Property(x => x.Completed).HasColumnName(@"Completed");
            builder.Property(x => x.EmployerContribution).HasColumnName(@"EmployerContribution");
            builder.Property(x => x.CompletionHoldBackExemptionCode).HasColumnName(@"CompletionHoldBackExemptionCode");
            builder.Property(x => x.AgreedPrice).HasColumnName(@"AgreedPrice");
            builder.Property(x => x.CourseStartDate).HasColumnName(@"CourseStartDate");

        }
    }
}
