using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data.Configuration
{
    public class CollectionPeriodToleranceModelConfiguration : IEntityTypeConfiguration<CollectionPeriodToleranceModel>
    {
        public void Configure(EntityTypeBuilder<CollectionPeriodToleranceModel> builder)
        {
            builder.ToTable("CollectionPeriodTolerance", "Metrics");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CollectionPeriod).HasColumnName("CollectionPeriod");
            builder.Property(x => x.AcademicYear).HasColumnName("AcademicYear");
            builder.Property(x => x.SubmissionToleranceLower).HasColumnName("SubmissionToleranceLower");
            builder.Property(x => x.SubmissionToleranceUpper).HasColumnName("SubmissionToleranceUpper");
            builder.Property(x => x.PeriodEndToleranceLower).HasColumnName("PeriodEndToleranceLower");
            builder.Property(x => x.PeriodEndToleranceUpper).HasColumnName("PeriodEndToleranceUpper");
        }
    }
}
