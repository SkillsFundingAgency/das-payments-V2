using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Data.DataLock
{
    public class DataLockEventNonPayablePeriodFailureModelConfiguration : IEntityTypeConfiguration<DataLockEventNonPayablePeriodFailureModel>
    {
        public void Configure(EntityTypeBuilder<DataLockEventNonPayablePeriodFailureModel> builder)
        {
            builder.ToTable("DataLockEventNonPayablePeriodFailures", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.DataLockFailure).HasColumnName(@"DataLockFailureId").HasColumnType("TinyInt").IsRequired();
            builder.Property(x => x.DataLockEventNonPayablePeriodId).HasColumnName(@"DataLockEventNonPayablePeriodId").IsRequired();
            builder.Property(x => x.ApprenticeshipId).HasColumnName(@"ApprenticeshipId");
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear");
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod");

            builder.HasOne(nppf => nppf.DataLockEventNonPayablePeriod)
                .WithMany(npp => npp.Failures)
                .HasPrincipalKey(npp => npp.DataLockEventNonPayablePeriodId)
                .HasForeignKey(nppf => nppf.DataLockEventNonPayablePeriodId);
        }
    }
}