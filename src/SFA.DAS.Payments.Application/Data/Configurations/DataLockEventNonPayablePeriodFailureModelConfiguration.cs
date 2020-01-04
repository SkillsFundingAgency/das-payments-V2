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
            builder.Property(x => x.DataLockFailure).HasColumnName(@"DataLockFailureId").IsRequired();
            builder.Property(x => x.DataLockEventNonPayablePeriodId).HasColumnName(@"DataLockEventNonPayablePeriod").IsRequired();
            builder.Property(x => x.ApprenticeshipId).HasColumnName(@"ApprenticeshipId").IsRequired();

            builder.HasOne(x => x.DataLockEventNonPayablePeriod).WithMany(dl => dl.Failures).HasPrincipalKey(x => x.DataLockEventNonPayablePeriodId);
            builder.HasOne(x => x.Apprenticeship).WithMany().HasForeignKey(x => x.ApprenticeshipId);
        }
    }
}