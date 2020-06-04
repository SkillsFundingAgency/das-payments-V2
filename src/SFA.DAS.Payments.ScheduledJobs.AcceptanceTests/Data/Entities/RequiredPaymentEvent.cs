using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data.Entities
{
    public class RequiredPaymentEvent
    {
        public long Id { get; set; }
        public Guid EventId { get; set; }
        public Guid EarningEventId { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public long Ukprn { get; set; }
        public byte ContractType { get; set; }
        public int TransactionType { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public decimal Amount { get; set; }
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }
        public short DeliveryPeriod { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public long LearnerUln { get; set; }
        public string LearningAimReference { get; set; }
        public int LearningAimProgrammeType { get; set; }
        public int LearningAimStandardCode { get; set; }
        public int LearningAimFrameworkCode { get; set; }
        public int LearningAimPathwayCode { get; set; }
        public string LearningAimFundingLineType { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public long JobId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public DateTime EarningsStartDate { get; set; }
        public short EarningsCompletionStatus { get; set; }
        public short EarningsNumberOfInstalments { get; set; }
    }

    public class RequiredPaymentEventConfiguration : IEntityTypeConfiguration<RequiredPaymentEvent>
    {
        public void Configure(EntityTypeBuilder<RequiredPaymentEvent> builder)
        {
            builder.ToTable("RequiredPaymentEvent", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.EventId).HasColumnName(@"EventId").IsRequired();
            builder.Property(x => x.EarningEventId).HasColumnName(@"EarningEventId").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn");
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier");
            builder.Property(x => x.ContractType).HasColumnName(@"ContractType");
            builder.Property(x => x.TransactionType).HasColumnName(@"TransactionType");
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage");
            builder.Property(x => x.Amount).HasColumnName(@"Amount");
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod");
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear");
            builder.Property(x => x.DeliveryPeriod).HasColumnName(@"DeliveryPeriod");
            builder.Property(x => x.LearnerReferenceNumber).HasColumnName(@"LearnerReferenceNumber");
            builder.Property(x => x.LearnerUln).HasColumnName(@"LearnerUln");
            builder.Property(x => x.LearningAimReference).HasColumnName(@"LearningAimReference");
            builder.Property(x => x.LearningAimProgrammeType).HasColumnName(@"LearningAimProgrammeType");
            builder.Property(x => x.LearningAimStandardCode).HasColumnName(@"LearningAimStandardCode");
            builder.Property(x => x.LearningAimFrameworkCode).HasColumnName(@"LearningAimFrameworkCode");
            builder.Property(x => x.LearningAimPathwayCode).HasColumnName(@"LearningAimPathwayCode");
            builder.Property(x => x.LearningAimFundingLineType).HasColumnName(@"LearningAimFundingLineType");
            builder.Property(x => x.IlrSubmissionDateTime).HasColumnName(@"IlrSubmissionDateTime");
            builder.Property(x => x.JobId).HasColumnName(@"JobId");
            builder.Property(x => x.EventTime).HasColumnName(@"EventTime");
            builder.Property(x => x.EarningsStartDate).HasColumnName(@"EarningsStartDate");
            builder.Property(x => x.EarningsCompletionStatus).HasColumnName(@"EarningsCompletionStatus");
            builder.Property(x => x.EarningsNumberOfInstalments).HasColumnName(@"EarningsNumberOfInstalments");
        }
    }
}