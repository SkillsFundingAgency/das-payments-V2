using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function.AcceptanceTests.Data
{
    public class TestSubmissionJobsDataContext : SubmissionJobsDataContext
    {
        public TestSubmissionJobsDataContext(DbContextOptions contextOptions) : base(contextOptions)
        { }

        public TestSubmissionJobsDataContext(string connectionString) : base(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
        { }

        public DbSet<JobsModel> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new JobsModelConfiguration());
        }
    }

    public class JobsModelConfiguration : IEntityTypeConfiguration<JobsModel>
    {
        public void Configure(EntityTypeBuilder<JobsModel> builder)
        {
            builder.ToTable("Job", "Payments2");
            builder.HasKey(x => x.JobId);
        }
    }

    public class JobsModel
    {
        public long JobId { get; set; }
        public byte JobType { get; set; }
        public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;
        public byte Status { get; set; }
        public long DcJobId { get; set; }
        public long Ukprn { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public bool? DcJobSucceeded { get; set; }
        public DateTime? IlrSubmissionTime { get; set; }
    }
}
