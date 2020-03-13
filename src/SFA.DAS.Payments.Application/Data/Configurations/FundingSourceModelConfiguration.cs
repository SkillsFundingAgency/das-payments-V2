using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class FundingSourceModelConfiguration : IEntityTypeConfiguration<FundingSourceModel>
    {
        public void Configure(EntityTypeBuilder<FundingSourceModel> builder)
        {
            builder.ToTable("FundingSourceEvent", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CompletionAmount).HasColumnName(@"EarningsCompletionAmount");
            builder.Property(x => x.CompletionStatus).HasColumnName(@"EarningsCompletionStatus");
            builder.Property(x => x.InstalmentAmount).HasColumnName(@"EarningsInstalmentAmount");
            builder.Property(x => x.StartDate).HasColumnName(@"EarningsStartDate");
            builder.Property(x => x.ActualEndDate).HasColumnName(@"EarningsActualEndDate");
            builder.Property(x => x.NumberOfInstalments).HasColumnName(@"EarningsNumberOfInstalments");
            builder.Property(x => x.PlannedEndDate).HasColumnName(@"EarningsPlannedEndDate");
            builder.Property(x => x.EventId).HasColumnName(@"EventId");
        }
    }
}