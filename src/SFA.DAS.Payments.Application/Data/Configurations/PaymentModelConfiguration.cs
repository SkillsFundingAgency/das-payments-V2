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

            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.EventId).HasColumnName(@"EventId").IsRequired();
            builder.Property(x => x.EventTime).HasColumnName(@"EventTime").IsRequired();
            builder.Property(x => x.FundingSourceEventId).HasColumnName(@"FundingSourceEventId").IsRequired();
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier");
            builder.Property(x => x.Amount).HasColumnName(@"Amount");
            builder.OwnsOne(p => p.CollectionPeriod, cp =>
            {
                cp.Property(x => x.Period).HasColumnName(@"CollectionPeriod").IsRequired();
                cp.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear").IsRequired();
            });
            builder.Property(x => x.DeliveryPeriod).HasColumnName("DeliveryPeriod").IsRequired();
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
            builder.Property(x => x.AccountId).HasColumnName(@"AccountId");
            builder.Property(x => x.TransferSenderAccountId).HasColumnName(@"TransferSenderAccountId");
            builder.Property(x => x.IlrSubmissionDateTime).HasColumnName(@"IlrSubmissionDateTime").IsRequired();
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage").IsRequired();
            builder.Property(x => x.JobId).HasColumnName(@"JobId").IsRequired();
            builder.Property(x => x.StartDate).HasColumnName(@"EarningsStartDate").IsRequired();
            builder.Property(x => x.PlannedEndDate).HasColumnName(@"EarningsPlannedEndDate");
            builder.Property(x => x.ActualEndDate).HasColumnName(@"EarningsActualEndDate");
            builder.Property(x => x.CompletionStatus).HasColumnName(@"EarningsCompletionStatus").IsRequired();
            builder.Property(x => x.CompletionAmount).HasColumnName(@"EarningsCompletionAmount").IsRequired();
            builder.Property(x => x.InstalmentAmount).HasColumnName(@"EarningsInstalmentAmount").IsRequired();
            builder.Property(x => x.NumberOfInstalments).HasColumnName(@"EarningsNumberOfInstalments").IsRequired();
        }
    }
}