using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class DataLockEventModelConfiguration : IEntityTypeConfiguration<DataLockEventModel>
    {
        public void Configure(EntityTypeBuilder<DataLockEventModel> builder)
        {
            builder.ToTable("DataLockEvent", "Payments2");
            builder.HasKey(x => x.EventId);
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
            builder.Property(x => x.IsPayable).HasColumnName(@"IsPayable");
            builder.Property(x => x.DataLockSource).HasColumnName(@"DataLockSourceId").HasDefaultValue(DataLockSource.Submission);

            builder.Ignore(x => x.LearningAimSequenceNumber);
            builder.Ignore(x => x.SfaContributionPercentage);
            builder.Ignore(x => x.IlrFileName);
            builder.Ignore(x => x.EventType);
            builder.Ignore(x => x.ActualEndDate);
            builder.Ignore(x => x.CompletionAmount);
            builder.Ignore(x => x.CompletionStatus);
            builder.Ignore(x => x.InstalmentAmount);
            builder.Ignore(x => x.PlannedEndDate);
            builder.Ignore(x => x.StartDate);
            builder.Ignore(x => x.NumberOfInstalments);

            builder.HasMany<DataLockEventNonPayablePeriodModel>(x => x.NonPayablePeriods).WithOne(y => y.DataLockEvent).HasForeignKey(p => p.DataLockEventId);
        }
    }
}