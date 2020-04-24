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
            builder.HasKey(x =>  x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id");
            builder.Property(x => x.EventId).HasColumnName(@"EventId");
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn");
            builder.Property(x => x.ContractType).HasColumnName(@"ContractType");
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod");
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear");
            builder.Property(x => x.LearnerReferenceNumber).HasColumnName(@"LearnerReferenceNumber");
            builder.Property(x => x.LearnerUln).HasColumnName(@"LearnerUln");
            builder.Property(x => x.LearningAimReference).HasColumnName(@"LearningAimReference");
            builder.Property(x => x.LearningAimProgrammeType).HasColumnName(@"LearningAimProgrammeType");
            builder.Property(x => x.LearningAimStandardCode).HasColumnName(@"LearningAimStandardCode");
            builder.Property(x => x.LearningAimFrameworkCode).HasColumnName(@"LearningAimFrameworkCode");
            builder.Property(x => x.LearningAimPathwayCode).HasColumnName(@"LearningAimPathwayCode");
            builder.Property(x => x.LearningAimFundingLineType).HasColumnName(@"LearningAimFundingLineType");
            builder.Property(x => x.AgreementId).HasColumnName(@"AgreementId");
            builder.Property(x => x.IlrSubmissionDateTime).HasColumnName(@"IlrSubmissionDateTime");
            builder.Property(x => x.JobId).HasColumnName(@"JobId");
            builder.Property(x => x.EventTime).HasColumnName(@"EventTime");
            builder.Property(x => x.LearningAimSequenceNumber).HasColumnName(@"LearningAimSequenceNumber");
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage");
            builder.Property(x => x.IlrFileName).HasColumnName(@"IlrFileName");
            builder.Property(x => x.EventType).HasColumnName(@"EventType");
          
            builder.Ignore(x => x.ActualEndDate);
            builder.Ignore(x => x.CompletionAmount);
            builder.Ignore(x => x.CompletionStatus);
            builder.Ignore(x => x.InstalmentAmount);
            builder.Ignore(x => x.PlannedEndDate);
            builder.Ignore(x => x.StartDate);
            builder.Ignore(x => x.NumberOfInstalments);

            builder.HasMany<EarningEventPeriodModel>(x => x.Periods)
                .WithOne(x => x.EarningEvent)
                .HasPrincipalKey(x => x.EventId)
                .HasForeignKey(x => x.EarningEventId);
            builder.HasMany<EarningEventPriceEpisodeModel>(x => x.PriceEpisodes).WithOne()
                .HasPrincipalKey(p => p.EventId)
                .HasForeignKey(pe => pe.EarningEventId);
        }
    }
}
