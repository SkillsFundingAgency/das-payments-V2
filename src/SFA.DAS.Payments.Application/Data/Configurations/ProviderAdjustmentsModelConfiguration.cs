using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    class ProviderAdjustmentsModelConfiguration : IEntityTypeConfiguration<ProviderAdjustmentModel>
    {
        public void Configure(EntityTypeBuilder<ProviderAdjustmentModel> builder)
        {
            builder.ToTable("ProviderAdjustmentPayments", "Payments2");
            builder.HasKey(e => new { e.Ukprn, e.SubmissionId, e.SubmissionCollectionPeriod, e.SubmissionAcademicYear, e.PaymentType, e.CollectionPeriodName});

            builder.Property(x => x.Ukprn).HasColumnName("Ukprn").IsRequired();
            builder.Property(x => x.SubmissionId).HasColumnName("SubmissionId").IsRequired();
            builder.Property(x => x.SubmissionCollectionPeriod).HasColumnName("SubmissionCollectionPeriod").IsRequired();
            builder.Property(x => x.SubmissionAcademicYear).HasColumnName("SubmissionAcademicYear").IsRequired();
            builder.Property(x => x.PaymentType).HasColumnName("PaymentType").IsRequired();
            builder.Property(x => x.CollectionPeriodName).HasColumnName("CollectionPeriodName").IsRequired();
            builder.Property(x => x.PaymentTypeName).HasColumnName("PaymentTypeName").IsRequired();
            builder.Property(x => x.Amount).HasColumnName("Amount").IsRequired();
            builder.Property(x => x.CollectionPeriodMonth).HasColumnName("CollectionPeriodMonth").IsRequired();
            builder.Property(x => x.CollectionPeriodYear).HasColumnName("CollectionPeriodYear").IsRequired();
        }
    }
}

