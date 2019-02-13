using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    class CommitmentModelConfiguration : IEntityTypeConfiguration<CommitmentModel>
    {
        public void Configure(EntityTypeBuilder<CommitmentModel> builder)
        {
            builder.ToTable("Commitment", "Payments2");
            builder.HasKey(x => x.CommitmentId);

            builder.Property(x => x.CommitmentId).HasColumnName(@"CommitmentId").IsRequired();
            builder.Property(x => x.SequenceId).HasColumnName(@"SequenceId").IsRequired();
            builder.Property(x => x.Uln).HasColumnName(@"Uln").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn").IsRequired();
            builder.Property(x => x.AccountId).HasColumnName(@"AccountId").IsRequired();
            builder.Property(x => x.AccountSequenceId).HasColumnName(@"AccountSequenceId").IsRequired();
            builder.Property(x => x.StartDate).HasColumnName(@"StartDate").IsRequired();
            builder.Property(x => x.EndDate).HasColumnName(@"EndDate").IsRequired();
            builder.Property(x => x.AgreedCost).HasColumnName(@"AgreedCost").IsRequired();
            builder.Property(x => x.StandardCode).HasColumnName(@"StandardCode");
            builder.Property(x => x.ProgrammeType).HasColumnName(@"ProgrammeType");
            builder.Property(x => x.FrameworkCode).HasColumnName(@"FrameworkCode");
            builder.Property(x => x.PathwayCode).HasColumnName(@"PathwayCode");
            builder.Property(x => x.PaymentStatus).HasColumnName(@"PaymentStatus").IsRequired();
            builder.Property(x => x.PaymentStatusDescription).HasColumnName(@"PaymentStatusDescription").IsRequired();
            builder.Property(x => x.Priority).HasColumnName(@"Priority").IsRequired();
            builder.Property(x => x.EffectiveFromDate).HasColumnName(@"EffectiveFromDate").IsRequired();
            builder.Property(x => x.EffectiveToDate).HasColumnName(@"EffectiveToDate");
            builder.Property(x => x.LegalEntityName).HasColumnName(@"LegalEntityName");
            builder.Property(x => x.TransferSendingEmployerAccountId).HasColumnName(@"TransferSendingEmployerAccountId");
            builder.Property(x => x.TransferApprovalDate).HasColumnName(@"TransferApprovalDate");
            builder.Property(x => x.PausedOnDate).HasColumnName(@"PausedOnDate");
            builder.Property(x => x.WithdrawnOnDate).HasColumnName(@"WithdrawnOnDate");
            builder.Property(x => x.AccountLegalEntityPublicHashedId).HasColumnName(@"AccountLegalEntityPublicHashedId");
        }
    }
}
