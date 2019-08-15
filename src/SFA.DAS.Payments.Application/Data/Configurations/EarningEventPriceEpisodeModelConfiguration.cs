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
            builder.ToTable("EarningEventPriceEpisodeModel", "Payments2");
            builder.Property(x => x.EarningEventId).HasColumnName(@"EarningEventId").IsRequired();
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier").IsRequired();
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage").IsRequired();
            builder.Property(x => x.TotalNegotiatedPrice1).HasColumnName(@"TotalNegotiatedPrice1").IsRequired();
            builder.Property(x => x.TotalNegotiatedPrice2).HasColumnName(@"TotalNegotiatedPrice2");
            builder.Property(x => x.TotalNegotiatedPrice3).HasColumnName(@"TotalNegotiatedPrice3");
            builder.Property(x => x.TotalNegotiatedPrice4).HasColumnName(@"TotalNegotiatedPrice4");
            builder.Property(x => x.StartDate).HasColumnName(@"StartDate").IsRequired();
            builder.Property(x => x.EffectiveTotalNegotiatedPriceStartDate).HasColumnName(@"EffectiveTotalNegotiatedPriceStartDate");
            builder.Property(x => x.PlannedEndDate).HasColumnName(@"[PlannedEndDate]").IsRequired();
            builder.Property(x => x.ActualEndDate).HasColumnName(@"ActualEndDate");
            builder.Property(x => x.NumberOfInstalments).HasColumnName(@"NumberOfInstalments").IsRequired();
            builder.Property(x => x.InstalmentAmount).HasColumnName(@"InstalmentAmount").IsRequired();
            builder.Property(x => x.CompletionAmount).HasColumnName(@"CompletionAmount").IsRequired();
            builder.Property(x => x.Completed).HasColumnName(@"Completed").IsRequired();
            builder.Property(x => x.EmployerContribution).HasColumnName(@"EmployerContribution");
            builder.Property(x => x.CompletionHoldBackExemptionCode).HasColumnName(@"CompletionHoldBackExemptionCode");
            builder.Property(x => x.AgreedPrice).HasColumnName(@"AgreedPrice");
            builder.Property(x => x.CourseStartDate).HasColumnName(@"CourseStartDate");

        }
    }
}
