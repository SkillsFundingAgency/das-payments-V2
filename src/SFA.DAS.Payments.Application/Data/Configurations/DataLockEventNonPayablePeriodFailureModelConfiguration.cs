using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Application.Data.Configurations
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

            builder.HasOne(x => x.DataLockEventNonPayablePeriod).WithMany().HasPrincipalKey(x => x.DataLockEventNonPayablePeriodId);
            builder.HasOne(x => x.Apprenticeship).WithMany().HasForeignKey(x => x.ApprenticeshipId);
        }
    }
}