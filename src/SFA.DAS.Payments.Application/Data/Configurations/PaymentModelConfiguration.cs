using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class PaymentModelConfiguration : IEntityTypeConfiguration<PaymentModel>
    {
        public void Configure(EntityTypeBuilder<PaymentModel> builder)
        {
            builder.ToTable("Payment", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"PaymentId").IsRequired();
            builder.Property(x => x.ExternalId).HasColumnName(@"ExternalId").IsRequired();
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier");
            builder.Property(x => x.Amount).HasColumnName(@"Amount");
            builder.OwnsOne(p => p.CollectionPeriod, cp =>
            {
                cp.Property(x => x.Name).HasColumnName(@"CollectionPeriodName").IsRequired();
                cp.Ignore(x => x.Period);
                cp.Ignore(x => x.AcademicYear);
            });
            builder.OwnsOne(p => p.DeliveryPeriod, cp =>
            {
                cp.Property(x => x.Month).HasColumnName(@"DeliveryPeriodMonth").IsRequired();
                cp.Property(x => x.Year).HasColumnName(@"DeliveryPeriodYear").IsRequired();
                cp.Property(x => x.Identifier).HasColumnName(@"DeliveryPeriodName").IsRequired();
                cp.Ignore(x => x.Period);
            });
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn").IsRequired();
            builder.Property(x => x.LearnerReferenceNumber).HasColumnName(@"LearnerReferenceNumber").IsRequired();
            builder.Property(x => x.LearnerUln).HasColumnName(@"LearnerUln").IsRequired();
            builder.Property(x => x.LearningAimReference).HasColumnName(@"LearningAimReference").IsRequired();
            builder.Property(x => x.LearningAimProgrammeType).HasColumnName(@"LearningAimProgrammeType").IsRequired();
            builder.Property(x => x.LearningAimStandardCode).HasColumnName(@"LearningAimStandardCode").IsRequired();
            builder.Property(x => x.LearningAimFrameworkCode).HasColumnName(@"LearningAimFrameworkCode").IsRequired();
            builder.Property(x => x.LearningAimPathwayCode).HasColumnName(@"LearningAimPathwayCode").IsRequired();
            builder.Property(x => x.LearningAimFundingLineType).HasColumnName(@"LearningAimFundingLineType").IsRequired();
            builder.Property(x => x.ContractType).HasColumnName(@"ContractType").IsRequired();
            builder.Property(x => x.TransactionType).HasColumnName(@"TransactionType").IsRequired();
            builder.Property(x => x.FundingSource).HasColumnName(@"FundingSource").IsRequired();
            builder.Property(x => x.IlrSubmissionDateTime).HasColumnName(@"IlrSubmissionDateTime").IsRequired();
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage").IsRequired();
            builder.Property(x => x.JobId).HasColumnName(@"JobId").IsRequired();
        }
    }
}