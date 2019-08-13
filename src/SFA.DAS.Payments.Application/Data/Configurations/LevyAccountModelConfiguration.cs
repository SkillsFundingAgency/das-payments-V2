using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    class LevyAccountModelConfiguration : IEntityTypeConfiguration<LevyAccountModel>
    {
        public void Configure(EntityTypeBuilder<LevyAccountModel> builder)
        {
            builder.ToTable("LevyAccount", "Payments2");
            builder.HasKey(x => new {x.AccountId});
            builder.Property(x => x.AccountId).HasColumnName(@"AccountId").IsRequired();
            builder.Property(x => x.AccountName).HasColumnName(@"AccountName").IsRequired();
            builder.Property(x => x.Balance).HasColumnName(@"Balance").IsRequired();
            builder.Property(x => x.IsLevyPayer).HasColumnName(@"IsLevyPayer").IsRequired();
            builder.Property(x => x.TransferAllowance).HasColumnName(@"TransferAllowance").IsRequired();
        }
    }
}

