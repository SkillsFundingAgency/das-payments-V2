using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class ReceivedDataLockEventConfiguration : IEntityTypeConfiguration<ReceivedDataLockEvent>
    {
        public void Configure(EntityTypeBuilder<ReceivedDataLockEvent> builder)
        {
            builder.ToTable("ReceivedDataLockEvent", "Payments2");

            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new
            {
                x.Ukprn,
                x.JobId
            });

            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn").IsRequired();
            builder.Property(x => x.JobId).HasColumnName(@"JobId").IsRequired();
            builder.Property(x => x.MessageType).HasColumnName(@"MessageType").IsRequired();
            builder.Property(x => x.Message).HasColumnName(@"Message").IsRequired();
        }
    }
}