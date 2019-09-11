using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Data.Configurations;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class TestPaymentsDataContext: PaymentsDataContext
    {
        public DbSet<TestModels.Provider> Providers { get; set; }

        public TestPaymentsDataContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ProviderModelConfiguration());
        }

        public TestModels.Provider LeastRecentlyUsed() => 
            Providers.OrderBy(x => x.LastUsed).FirstOrDefault()
            ?? throw new InvalidOperationException("There are no UKPRNs available in the well-known Providers pool.");

        public void ClearPaymentsData(long ukprn)
        {
            const string deleteUkprnData = @"
delete from Payments2.ApprenticeshipPriceEpisode where ApprenticeshipId in 
	(select Id from Payments2.Apprenticeship where Ukprn = {0})

delete from Payments2.ApprenticeshipDuplicate where Ukprn = {0}

delete from Payments2.LevyAccount where AccountId in
	(select AccountId from Payments2.Apprenticeship where Ukprn = {0})

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

            Database.ExecuteSqlCommand(deleteUkprnData, ukprn);
        }

        public void ClearApprenticeshipData(long apprenticeshipId)
        {
            const string deleteApprenticeshipData = @"
delete from Payments2.Apprenticeship where id = {0}
delete from Payments2.ApprenticeshipPriceEpisode where ApprenticeshipId = {0}
";
            Database.ExecuteSqlCommand(deleteApprenticeshipData, apprenticeshipId);
        }
    }
}