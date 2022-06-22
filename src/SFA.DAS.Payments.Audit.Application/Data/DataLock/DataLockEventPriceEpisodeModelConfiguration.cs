using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Data.DataLock
{
    public class DataLockEventPriceEpisodeModelConfiguration : IEntityTypeConfiguration<DataLockEventPriceEpisodeModel>
    {
        public void Configure(EntityTypeBuilder<DataLockEventPriceEpisodeModel> builder)
        {
            builder.ToTable("DataLockEventPriceEpisode", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.DataLockEventId).HasColumnName(@"DataLockEventId").IsRequired();
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
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear");
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod");
        }
    }
}