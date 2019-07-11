using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class DataLockFailureModelConfiguration : IEntityTypeConfiguration<DataLockFailureModel>
    {
        public void Configure(EntityTypeBuilder<DataLockFailureModel> builder)
        {
            builder.ToTable("DataLockFailure", "Payments2");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();

            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn").IsRequired();
            builder.Property(x => x.LearnerReferenceNumber).HasColumnName(@"LearnerReferenceNumber").IsRequired();
            builder.Property(x => x.LearnerUln).HasColumnName(@"LearnerUln").IsRequired();
            builder.Property(x => x.LearningAimFrameworkCode).HasColumnName(@"LearningAimFrameworkCode").IsRequired();
            builder.Property(x => x.LearningAimPathwayCode).HasColumnName(@"LearningAimPathwayCode").IsRequired();
            builder.Property(x => x.LearningAimProgrammeType).HasColumnName(@"LearningAimProgrammeType").IsRequired();
            builder.Property(x => x.LearningAimStandardCode).HasColumnName(@"LearningAimStandardCode").IsRequired();
            builder.Property(x => x.LearningAimReference).HasColumnName(@"LearningAimReference").IsRequired();

            builder.Property(x => x.DeliveryPeriod).HasColumnName(@"DeliveryPeriod").IsRequired();
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod").IsRequired();
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear").IsRequired();
            builder.Property(x => x.TransactionType).HasColumnName(@"TransactionType").IsRequired();

            builder.Property(x => x.EarningPeriod).HasColumnName(@"EarningPeriod").IsRequired();
        }
    }
}