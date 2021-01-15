using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data.Configuration
{
    public class ProviderPeriodEndSummaryModelConfiguration: IEntityTypeConfiguration<ProviderPeriodEndSummaryModel>
    {
        public void Configure(EntityTypeBuilder<ProviderPeriodEndSummaryModel> builder)
        {
             builder.ToTable("ProviderPeriodEndSummary", "Metrics");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn");
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear");
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod");
            builder.Ignore(x => x.DataLockTypeCounts);
            builder.Property(x => x.JobId).HasColumnName(@"JobId");
            builder.Property(x => x.Percentage).HasColumnName(@"Percentage");
            builder.OwnsOne(x => x.PaymentMetrics, amounts =>
            {
                amounts.Property(x => x.ContractType1).HasColumnName(@"ContractType1");
                amounts.Property(x => x.ContractType2).HasColumnName(@"ContractType2");
                amounts.Property(x => x.DifferenceContractType1).HasColumnName(@"DifferenceContractType1");
                amounts.Property(x => x.DifferenceContractType2).HasColumnName(@"DifferenceContractType2");
                amounts.Property(x => x.PercentageContractType1).HasColumnName(@"PercentageContractType1");
                amounts.Property(x => x.PercentageContractType2).HasColumnName(@"PercentageContractType2");
                amounts.Ignore(x => x.DifferenceTotal);
                amounts.Ignore(x => x.Percentage);
                amounts.Ignore(x => x.Total);
            });
            builder.OwnsOne(x => x.DcEarnings, amounts =>
            {
                amounts.Property(x => x.ContractType1).HasColumnName(@"EarningsDCContractType1");
                amounts.Property(x => x.ContractType2).HasColumnName(@"EarningsDCContractType2");
                amounts.Ignore(x => x.Total);
            });
           
            builder.OwnsOne(x => x.Payments, amounts =>
            {
                amounts.Property(x => x.ContractType1).HasColumnName(@"PaymentsContractType1");
                amounts.Property(x => x.ContractType2).HasColumnName(@"PaymentsContractType2");
                amounts.Ignore(x => x.Total);
            });
            builder.Property(x => x.AdjustedDataLockedEarnings).HasColumnName(@"AdjustedDataLockedEarnings");
            builder.Property(x => x.AlreadyPaidDataLockedEarnings).HasColumnName(@"AlreadyPaidDataLockedEarnings");
            builder.Property(x => x.TotalDataLockedEarnings).HasColumnName(@"TotalDataLockedEarnings");
            builder.OwnsOne(x => x.HeldBackCompletionPayments, amounts =>
            {
                amounts.Property(x => x.ContractType1).HasColumnName(@"HeldBackCompletionPaymentsContractType1");
                amounts.Property(x => x.ContractType2).HasColumnName(@"HeldBackCompletionPaymentsContractType2");
                amounts.Ignore(x => x.Total);
            });
            builder.OwnsOne(x => x.YearToDatePayments, amounts =>
            {
                amounts.Property(x => x.ContractType1).HasColumnName(@"PaymentsYearToDateContractType1");
                amounts.Property(x => x.ContractType2).HasColumnName(@"PaymentsYearToDateContractType2");
                amounts.Ignore(x => x.Total);
            });

            builder.HasMany(x => x.FundingSourceAmounts).WithOne(e => e.ProviderPeriodEndSummary).HasForeignKey(e => e.ProviderPeriodEndSummaryId);
            builder.HasMany(x => x.TransactionTypeAmounts).WithOne(d => d.ProviderPeriodEndSummary).HasForeignKey(d => d.ProviderPeriodEndSummaryId);
        
        }
    }
}