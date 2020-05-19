using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class FundingSourceEventModelConfiguration : IEntityTypeConfiguration<FundingSourceEventModel>
    {
        public void Configure(EntityTypeBuilder<FundingSourceEventModel> builder)
        {
            builder.ToTable("Payment", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id");
            builder.Property(x => x.EventId).HasColumnName(@"EventId");
            builder.Property(x => x.EarningEventId).HasColumnName(@"EarningEventId");
            builder.Property(x => x.RequiredPaymentEventId).HasColumnName(@"RequiredPaymentEventId");
            builder.Property(x => x.EventTime).HasColumnName(@"EventTime");
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier");
            builder.Property(x => x.Amount).HasColumnName(@"Amount");
            builder.OwnsOne(p => p.CollectionPeriod, cp =>
            {
                cp.Property(x => x.Period).HasColumnName(@"CollectionPeriod");
                cp.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear");
            });
            builder.Property(x => x.DeliveryPeriod).HasColumnName("DeliveryPeriod");
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn");
            builder.Property(x => x.LearnerReferenceNumber).HasColumnName(@"LearnerReferenceNumber");
            builder.Property(x => x.LearnerUln).HasColumnName(@"LearnerUln");
            builder.Property(x => x.LearningAimReference).HasColumnName(@"LearningAimReference");
            builder.Property(x => x.LearningAimProgrammeType).HasColumnName(@"LearningAimProgrammeType");
            builder.Property(x => x.LearningAimStandardCode).HasColumnName(@"LearningAimStandardCode");
            builder.Property(x => x.LearningAimFrameworkCode).HasColumnName(@"LearningAimFrameworkCode");
            builder.Property(x => x.LearningAimPathwayCode).HasColumnName(@"LearningAimPathwayCode");
            builder.Property(x => x.LearningAimFundingLineType).HasColumnName(@"LearningAimFundingLineType");
            builder.Property(x => x.ContractType).HasColumnName(@"ContractType");
            builder.Property(x => x.TransactionType).HasColumnName(@"TransactionType");
            builder.Property(x => x.FundingSource).HasColumnName(@"FundingSourceType");
            builder.Property(x => x.AccountId).HasColumnName(@"AccountId");
            builder.Property(x => x.TransferSenderAccountId).HasColumnName(@"TransferSenderAccountId");
            builder.Property(x => x.IlrSubmissionDateTime).HasColumnName(@"IlrSubmissionDateTime");
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage");
            builder.Property(x => x.JobId).HasColumnName(@"JobId");
            builder.Property(x => x.StartDate).HasColumnName(@"EarningsStartDate");
            builder.Property(x => x.PlannedEndDate).HasColumnName(@"EarningsPlannedEndDate");
            builder.Property(x => x.ActualEndDate).HasColumnName(@"EarningsActualEndDate");
            builder.Property(x => x.CompletionStatus).HasColumnName(@"EarningsCompletionStatus");
            builder.Property(x => x.CompletionAmount).HasColumnName(@"EarningsCompletionAmount");
            builder.Property(x => x.InstalmentAmount).HasColumnName(@"EarningsInstalmentAmount");
            builder.Property(x => x.NumberOfInstalments).HasColumnName(@"EarningsNumberOfInstalments");
            builder.Property(x => x.AgreementId).HasColumnName(@"AgreementId");
            builder.Property(x => x.LearningStartDate).HasColumnName(@"LearningStartDate");
            builder.Property(x => x.ApprenticeshipId).HasColumnName(@"ApprenticeshipId");
            builder.Property(x => x.ApprenticeshipPriceEpisodeId).HasColumnName(@"ApprenticeshipPriceEpisodeId");
            builder.Property(x => x.ApprenticeshipEmployerType).HasColumnName(@"ApprenticeshipEmployerType");
        }
    }
}