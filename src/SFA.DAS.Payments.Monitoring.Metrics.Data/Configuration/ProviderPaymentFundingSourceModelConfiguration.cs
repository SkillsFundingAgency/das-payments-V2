using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data.Configuration
{
    public class ProviderPaymentFundingSourceModelConfiguration: IEntityTypeConfiguration<ProviderPaymentFundingSourceModel>
    {
        public void Configure(EntityTypeBuilder<ProviderPaymentFundingSourceModel> builder)
        {
            builder.ToTable("ProviderPaymentFundingSource", "Metrics");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProviderPeriodEndSummaryId).HasColumnName(@"ProviderPeriodEndSummaryId").IsRequired();
            builder.Property(x => x.ContractType).HasColumnName(@"ContractType").IsRequired();
            builder.Property(x => x.FundingSource1).HasColumnName(@"FundingSource1").IsRequired();
            builder.Property(x => x.FundingSource2).HasColumnName(@"FundingSource2").IsRequired();
            builder.Property(x => x.FundingSource3).HasColumnName(@"FundingSource3").IsRequired();
            builder.Property(x => x.FundingSource4).HasColumnName(@"FundingSource4").IsRequired();
            builder.Property(x => x.FundingSource5).HasColumnName(@"FundingSource5").IsRequired();
            builder.Ignore(x => x.Total);
            builder.HasOne(x => x.ProviderPeriodEndSummary).WithMany(x => x.FundingSourceAmounts)
                .HasForeignKey(x => x.ProviderPeriodEndSummaryId);
        }
    }
}