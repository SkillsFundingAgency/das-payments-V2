﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Configuration
{
    public class JobModelConfiguration:  IEntityTypeConfiguration<JobModel>
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