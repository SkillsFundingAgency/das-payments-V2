using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class ApprenticeshipModelConfiguration : IEntityTypeConfiguration<ApprenticeshipModel>
    {
        public void Configure(EntityTypeBuilder<ApprenticeshipModel> builder)
        {
            builder.ToTable("Apprenticeship", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.AccountId).HasColumnName(@"AccountId").IsRequired();
            builder.Property(x => x.AgreementId).HasColumnName(@"AgreementId");
            builder.Property(x => x.AgreedOnDate).HasColumnName(@"AgreedOnDate").IsRequired();
            builder.Property(x => x.Uln).HasColumnName(@"Uln").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn").IsRequired();
            builder.Property(x => x.EstimatedStartDate).HasColumnName(@"EstimatedStartDate").IsRequired();
            builder.Property(x => x.EstimatedEndDate).HasColumnName(@"EstimatedEndDate").IsRequired();
            builder.Property(x => x.Priority).HasColumnName(@"Priority").IsRequired();
            builder.Property(x => x.StandardCode).HasColumnName(@"StandardCode");
            builder.Property(x => x.ProgrammeType).HasColumnName(@"ProgrammeType");
            builder.Property(x => x.FrameworkCode).HasColumnName(@"FrameworkCode");
            builder.Property(x => x.PathwayCode).HasColumnName(@"PathwayCode");
            builder.Property(x => x.LegalEntityName).HasColumnName(@"LegalEntityName");
            builder.Property(x => x.TransferSendingEmployerAccountId).HasColumnName(@"TransferSendingEmployerAccountId");
            builder.Property(x => x.StopDate).HasColumnName(@"StopDate");
            builder.Property(x => x.Status).HasColumnName(@"Status").IsRequired();
            builder.Property(x => x.IsLevyPayer).HasColumnName(@"IsLevyPayer").IsRequired();
            builder.Property(x => x.ApprenticeshipEmployerType).HasColumnName(@"ApprenticeshipEmployerType").IsRequired();
            builder.Property(x => x.CreationDate).HasDefaultValue();
            builder.HasMany<ApprenticeshipPriceEpisodeModel>(x => x.ApprenticeshipPriceEpisodes).WithOne().HasForeignKey(p => p.ApprenticeshipId);
            builder.HasMany<ApprenticeshipPauseModel>(x => x.ApprenticeshipPauses).WithOne().HasForeignKey(ap => ap.ApprenticeshipId);
        }
    }
}
