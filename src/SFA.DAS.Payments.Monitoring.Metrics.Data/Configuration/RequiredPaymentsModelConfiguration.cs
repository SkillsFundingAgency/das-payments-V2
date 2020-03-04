using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data.Configuration
{
    public class RequiredPaymentsModelConfiguration : IEntityTypeConfiguration<RequiredPaymentsModel>
    {
        public void Configure(EntityTypeBuilder<RequiredPaymentsModel> builder)
        {
            builder.ToTable("ProviderRequiredPayment", "Metrics");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SubmissionSummaryId).HasColumnName(@"SubmissionSummaryId").IsRequired();
            builder.OwnsOne(x => x.Amounts, amounts =>
            {
                amounts.Property(x => x.ContractType).HasColumnName("ContractType");
                amounts.Property(x => x.TransactionType1).HasColumnName("TransactionType1");
                amounts.Property(x => x.TransactionType2).HasColumnName("TransactionType2");
                amounts.Property(x => x.TransactionType3).HasColumnName("TransactionType3");
                amounts.Property(x => x.TransactionType4).HasColumnName("TransactionType4");
                amounts.Property(x => x.TransactionType5).HasColumnName("TransactionType5");
                amounts.Property(x => x.TransactionType6).HasColumnName("TransactionType6");
                amounts.Property(x => x.TransactionType7).HasColumnName("TransactionType7");
                amounts.Property(x => x.TransactionType8).HasColumnName("TransactionType8");
                amounts.Property(x => x.TransactionType9).HasColumnName("TransactionType9");
                amounts.Property(x => x.TransactionType10).HasColumnName("TransactionType10");
                amounts.Property(x => x.TransactionType11).HasColumnName("TransactionType11");
                amounts.Property(x => x.TransactionType12).HasColumnName("TransactionType12");
                amounts.Property(x => x.TransactionType13).HasColumnName("TransactionType13");
                amounts.Property(x => x.TransactionType14).HasColumnName("TransactionType14");
                amounts.Property(x => x.TransactionType15).HasColumnName("TransactionType15");
                amounts.Property(x => x.TransactionType16).HasColumnName("TransactionType16");
                amounts.Ignore(x => x.Total);
            });
            builder.HasOne(x => x.SubmissionSummary).WithMany(x => x.RequiredPaymentsMetrics)
                .HasForeignKey(x => x.SubmissionSummaryId);
        }
    }
}