using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class DeferredApprenticeshipEventModelConfiguration : IEntityTypeConfiguration<DeferredApprovalsEventModel>
    {
        public void Configure(EntityTypeBuilder<DeferredApprovalsEventModel> builder)
        {
            builder.ToTable("DeferredApprenticeshipEvent", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.EventTime).HasColumnName(@"EventTime").IsRequired();
            builder.Property(x => x.EventBody).HasColumnName(@"EventBody").IsRequired();
        }
    }
}