using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class EmployerProviderPriorityModelConfiguration : IEntityTypeConfiguration<EmployerProviderPriorityModel>
    {
        public void Configure(EntityTypeBuilder<EmployerProviderPriorityModel> builder)
        {
            builder.ToTable("EmployerProviderPriority", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn").IsRequired();
            builder.Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").IsRequired();
            builder.Property(x => x.Order).HasColumnName(@"Order").IsRequired();
        }
    }
}
