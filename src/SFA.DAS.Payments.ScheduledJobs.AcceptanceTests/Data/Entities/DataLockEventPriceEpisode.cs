using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data.Entities
{
    public class DataLockEventPriceEpisode
    {
        public long Id { get; set; }
        public Guid DataLockEventId { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public decimal TotalNegotiatedPrice1 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public int NumberOfInstalments { get; set; }
        public decimal InstalmentAmount { get; set; }
        public decimal CompletionAmount { get; set; }
        public bool Completed { get; set; }
    }

    public class DataLockEventPriceEpisodeConfiguration : IEntityTypeConfiguration<DataLockEventPriceEpisode>
    {
        public void Configure(EntityTypeBuilder<DataLockEventPriceEpisode> builder)
        {
            builder.ToTable("DataLockEventPriceEpisode", "Payments2");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.DataLockEventId).HasColumnName(@"DataLockEventId").IsRequired();
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier").IsRequired();
            builder.Property(x => x.SfaContributionPercentage).HasColumnName(@"SfaContributionPercentage");
            builder.Property(x => x.TotalNegotiatedPrice1).HasColumnName(@"TotalNegotiatedPrice1");
            builder.Property(x => x.StartDate).HasColumnName(@"StartDate");
            builder.Property(x => x.PlannedEndDate).HasColumnName(@"PlannedEndDate");
            builder.Property(x => x.NumberOfInstalments).HasColumnName(@"NumberOfInstalments");
            builder.Property(x => x.InstalmentAmount).HasColumnName(@"InstalmentAmount");
            builder.Property(x => x.CompletionAmount).HasColumnName(@"CompletionAmount");
            builder.Property(x => x.Completed).HasColumnName(@"Completed");
        }
    }

}