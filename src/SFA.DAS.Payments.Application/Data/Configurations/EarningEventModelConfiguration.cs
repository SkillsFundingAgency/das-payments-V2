using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class EarningEventModelConfiguration : IEntityTypeConfiguration<EarningEventModel>
    {
        public void Configure(EntityTypeBuilder<EarningEventModel> builder)
        {
            builder.ToTable("EarningEvent", "Payments2");
            builder.HasKey(x =>  x.EventId);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.EventId).HasColumnName(@"EventId").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn").IsRequired();
            builder.Property(x => x.ContractType).HasColumnName(@"ContractType").IsRequired();
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod").IsRequired();
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear").IsRequired();
            builder.Property(x => x.LearnerReferenceNumber).HasColumnName(@"LearnerReferenceNumber").IsRequired();
            builder.Property(x => x.LearnerUln).HasColumnName(@"LearnerUln").IsRequired();
            builder.Property(x => x.LearningAimReference).HasColumnName(@"LearningAimReference").IsRequired();
            builder.Property(x => x.LearningAimProgrammeType).HasColumnName(@"LearningAimProgrammeType").IsRequired();
            builder.Property(x => x.LearningAimStandardCode).HasColumnName(@"LearningAimStandardCode").IsRequired();
            builder.Property(x => x.LearningAimFrameworkCode).HasColumnName(@"LearningAimFrameworkCode").IsRequired();
            builder.Property(x => x.LearningAimPathwayCode).HasColumnName(@"LearningAimPathwayCode").IsRequired();
            builder.Property(x => x.LearningAimFundingLineType).HasColumnName(@"LearningAimFundingLineType").IsRequired();
            builder.Property(x => x.AgreementId).HasColumnName(@"AgreementId");
            builder.Property(x => x.IlrSubmissionDateTime).HasColumnName(@"IlrSubmissionDateTime").IsRequired();
            builder.Property(x => x.JobId).HasColumnName(@"JobId").IsRequired();
            builder.Property(x => x.EventTime).HasColumnName(@"EventTime").IsRequired();
            builder.Property(x => x.LearningAimSequenceNumber).HasColumnName(@"LearningAimSequenceNumber");
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage");
            builder.Property(x => x.IlrFileName).HasColumnName(@"IlrFileName");
            builder.Ignore(x => x.ActualEndDate);
            builder.Ignore(x => x.CompletionAmount);
            builder.Ignore(x => x.CompletionStatus);
            builder.Ignore(x => x.InstalmentAmount);
            builder.Ignore(x => x.PlannedEndDate);
            builder.Ignore(x => x.StartDate);
            builder.Ignore(x => x.NumberOfInstalments);

            builder.HasMany<EarningEventPeriodModel>(x => x.Periods).WithOne().HasForeignKey(p => p.EarningEventId);
            builder.HasMany<EarningEventPriceEpisodeModel>(x => x.PriceEpisodes).WithOne().HasForeignKey(p => p.EarningEventId);
        }
    }
}
