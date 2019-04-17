using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.TestModels;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;
using System;
using System.Linq;

namespace SFA.DAS.Payments.AcceptanceTests.Services
{
    internal class UkprnService : DbContext, IUkprnService
    {
        public UkprnService(DbContextOptions<UkprnService> options) : base(options)
        {
        }

        public int GenerateUkprn()
        {
            var provider = LeastRecentlyUsed();
            provider.Use();
            SaveChanges();

            ClearPaymentsData(provider);

            return provider.Ukprn;
        }

        private Provider LeastRecentlyUsed() =>
            Providers.OrderBy(x => x.LastUsed).FirstOrDefault()
            ?? throw new InvalidOperationException("There are no UKPRNs available in the well-known Providers pool.");

        private void ClearPaymentsData(Provider provider)
        {
            const string DeleteUkprnData = @"
delete from Payments2.ApprenticeshipPriceEpisode where ApprenticeshipId in 
	(select Id from Payments2.Apprenticeship where Ukprn = {0})

delete from Payments2.Apprenticeship where Ukprn = {0}

delete from Payments2.EarningEventPeriod where EarningEventId in 
	(select EventId from Payments2.EarningEvent where Ukprn = {0})

delete from Payments2.EarningEventPriceEpisode where EarningEventId in 
	(select EventId from Payments2.EarningEvent where Ukprn = {0})

delete from Payments2.EarningEvent where Ukprn = {0}

delete from Payments2.FundingSourceEvent where Ukprn = {0}

delete from Payments2.RequiredPaymentEvent where Ukprn = {0}

delete from Payments2.Payment where Ukprn = {0}
";

            Database.ExecuteSqlCommand(DeleteUkprnData, provider.Ukprn);
        }

        private DbSet<Provider> Providers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.Entity<Provider>().ToTable("TestingProvider");
            modelBuilder.Entity<Provider>().HasKey(x => x.Ukprn);
            modelBuilder.Entity<Provider>().HasIndex(x => x.LastUsed);
        }
    }
}
