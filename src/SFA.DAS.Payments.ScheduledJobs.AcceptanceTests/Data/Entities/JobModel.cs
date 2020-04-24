using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data.Entities
{
    public class JobModel
    {
        public long Id { get; set; }
        public JobType JobType { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public JobStatus Status { get; set; }
        public long? DcJobId { get; set; }
        public long? Ukprn { get; set; }
        public DateTime? IlrSubmissionTime { get; set; }
        public int? LearnerCount { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public DateTimeOffset? DataLocksCompletionTime { get; set; }
        public bool? DcJobSucceeded { get; set; }
        public DateTimeOffset? DcJobEndTime { get; set; }
    }

    public enum JobStatus : byte
    {
        InProgress = 1,
        Completed = 2,
        CompletedWithErrors = 3,
        TimedOut = 4,
        DcTasksFailed = 5
    }

    public enum JobType : byte
    {
        EarningsJob = 1,
        PeriodEndStartJob = 2,
        ComponentAcceptanceTestEarningsJob = 3,
        ComponentAcceptanceTestMonthEndJob = 4,
        PeriodEndRunJob = 5,
        PeriodEndStopJob = 6
    }

    public class JobModelConfiguration : IEntityTypeConfiguration<JobModel>
    {
        public void Configure(EntityTypeBuilder<JobModel> builder)
        {
            builder.ToTable("Job", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"JobId").IsRequired();
            builder.Property(x => x.StartTime).HasColumnName(@"StartTime").IsRequired();
            builder.Property(x => x.EndTime).HasColumnName(@"EndTime");
            builder.Property(x => x.Status).HasColumnName(@"Status");
            builder.Property(x => x.DcJobId).HasColumnName(@"DCJobId").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn");
            builder.Property(x => x.IlrSubmissionTime).HasColumnName(@"IlrSubmissionTime");
            builder.Property(x => x.LearnerCount).HasColumnName(@"LearnerCount");
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear");
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod");
            builder.Property(x => x.DataLocksCompletionTime).HasColumnName(@"DataLocksCompletionTime");
            builder.Property(x => x.DcJobSucceeded).HasColumnName(@"DCJobSucceeded");
            builder.Property(x => x.DcJobEndTime).HasColumnName(@"DCJobEndTime");
        }
    }
}