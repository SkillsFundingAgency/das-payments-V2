using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class LevyTransactionModelConfiguration : IEntityTypeConfiguration<LevyTransactionModel>
    {
        public void Configure(EntityTypeBuilder<LevyTransactionModel> builder)
        {
            builder.ToTable("FundingSourceLevyTransaction", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id");
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn");
            builder.Property(x => x.Amount).HasColumnName(@"Amount");
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear");
            builder.Property(x => x.AccountId).HasColumnName(@"AccountId");
            builder.Property(x => x.TransferSenderAccountId).HasColumnName(@"TransferSenderAccountId");
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod");
            builder.Property(x => x.DeliveryPeriod).HasColumnName(@"DeliveryPeriod");
            builder.Property(x => x.EarningEventId).HasColumnName(@"EarningEventId");
            builder.Property(x => x.JobId).HasColumnName(@"JobId");
            builder.Property(x => x.MessagePayload).HasColumnName(@"MessagePayload");
            builder.Property(x => x.MessageType).HasColumnName(@"MessageType");
            builder.Property(x => x.IlrSubmissionDateTime).HasColumnName(@"IlrSubmissionDateTime");
            builder.Property(x => x.FundingAccountId).HasColumnName(@"FundingAccountId");
            builder.Property(x => x.TransactionType).HasColumnName(@"TransactionType").IsRequired();
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage").IsRequired();
            builder.Property(x => x.LearnerReferenceNumber).HasColumnName(@"LearnerReferenceNumber").IsRequired();
            builder.Property(x => x.LearningAimReference).HasColumnName(@"LearningAimReference").IsRequired();
            builder.Property(x => x.LearningAimProgrammeType).HasColumnName(@"LearningAimProgrammeType").IsRequired();
            builder.Property(x => x.LearningAimStandardCode).HasColumnName(@"LearningAimStandardCode").IsRequired();
            builder.Property(x => x.LearningAimFrameworkCode).HasColumnName(@"LearningAimFrameworkCode").IsRequired();
            builder.Property(x => x.LearningAimPathwayCode).HasColumnName(@"LearningAimPathwayCode").IsRequired();
            builder.Property(x => x.LearningAimFundingLineType).HasColumnName(@"LearningAimFundingLineType").IsRequired();
            builder.Property(x => x.LearningStartDate).HasColumnName(@"LearningStartDate");
            builder.Property(x => x.ApprenticeshipId).HasColumnName(@"ApprenticeshipId");
            builder.Property(x => x.ApprenticeshipEmployerType).HasColumnName(@"ApprenticeshipEmployerType").IsRequired();
        }
    }
}