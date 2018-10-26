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
            builder.Property(x => x.CollectionPeriod.Name).HasColumnName(@"CollectionPeriodName").IsRequired();
            builder.Property(x => x.CollectionPeriod.Month).HasColumnName(@"CollectionPeriodMonth").IsRequired();
            builder.Property(x => x.CollectionPeriod.Year).HasColumnName(@"CollectionPeriodYear").IsRequired();
            builder.Property(x => x.DeliveryPeriod.Month).HasColumnName(@"DeliveryPeriodMonth").IsRequired();
            builder.Property(x => x.DeliveryPeriod.Year).HasColumnName(@"DeliveryPeriodYear").IsRequired();
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
            //builder.Property(x => x.Earnings.StartDate).HasColumnName(@"EarningsStartDate").IsRequired();
            //builder.Property(x => x.Earnings.PlannedEndDate).HasColumnName(@"EarningsPlannedEndDate").IsRequired();
            //builder.Property(x => x.Earnings.ActualEndDate).HasColumnName(@"EarningsActualEndDate");
            //builder.Property(x => x.Earnings.CompletionStatus).HasColumnName(@"EarningsCompletionStatus").IsRequired();
            //builder.Property(x => x.Earnings.CompletionAmount).HasColumnName(@"EarningsCompletionAmount").IsRequired();
            //builder.Property(x => x.Earnings.InstalmentAmount).HasColumnName(@"EarningsInstalmentAmount").IsRequired();
            //builder.Property(x => x.Earnings.NumberOfInstalments).HasColumnName(@"EarningsNumberOfInstalments").IsRequired();
            builder.Property(x => x.JobId).HasColumnName(@"JobId").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate");
        }
    }
}