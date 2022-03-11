using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class LevyAccountAuditModelConfiguration : IEntityTypeConfiguration<LevyAccountAuditModel>
    {
        public void Configure(EntityTypeBuilder<LevyAccountAuditModel> builder)
        {
            builder.ToTable("LevyAccountAudit", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("Id").IsRequired();
            builder.Property(x => x.AccountId).HasColumnName("AccountId").IsRequired();
            builder.Property(x => x.AcademicYear).HasColumnName("AcademicYear").IsRequired();
            builder.Property(x => x.CollectionPeriod).HasColumnName("CollectionPeriod").IsRequired();
            builder.Property(x => x.SourceLevyAccountBalance).HasColumnName("SourceLevyAccountBalance").IsRequired();
            builder.Property(x => x.AdjustedLevyAccountBalance).HasColumnName("AdjustedLevyAccountBalance").IsRequired();
            builder.Property(x => x.SourceTransferAllowance).HasColumnName("SourceTransferAllowance").IsRequired();
            builder.Property(x => x.AdjustedTransferAllowance).HasColumnName("AdjustedTransferAllowance").IsRequired();
            builder.Property(x => x.IsLevyPayer).HasColumnName("IslevyPayer").IsRequired();
        }
    }
}
