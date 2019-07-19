using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class SubmittedPriceEpisodeModelConfiguration : IEntityTypeConfiguration<SubmittedPriceEpisodeModel>
    {
        public void Configure(EntityTypeBuilder<SubmittedPriceEpisodeModel> builder)
        {
            builder.ToTable("SubmittedPriceEpisode", "Payments2");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.Ukprn, 
                x.LearnerReferenceNumber
            });

            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn").IsRequired();
            builder.Property(x => x.LearnerReferenceNumber).HasColumnName(@"LearnerReferenceNumber").IsRequired();
            builder.Property(x => x.PriceEpisodeIdentifier).HasColumnName(@"PriceEpisodeIdentifier").IsRequired();
            builder.Property(x => x.IlrDetails).HasColumnName(@"IlrDetails").IsRequired();
        }
    }
}