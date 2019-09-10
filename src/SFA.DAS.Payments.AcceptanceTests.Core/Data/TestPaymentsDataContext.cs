using System;
using System.Linq;
using System.Threading.Tasks;
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

            Database.ExecuteSqlCommand(DeleteUkprnData, ukprn);
        }

        public Task<int> ClearPaymentsDataAsync(long ukprn)
        {
           return Database.ExecuteSqlCommandAsync(DeleteUkprnData, ukprn);
        }

        private const string DeleteUkprnData = @"
delete from Payments2.LevyAccount where AccountId in
	(select AccountId from Payments2.Apprenticeship where Ukprn = {0})

delete from Payments2.ApprenticeshipPriceEpisode where ApprenticeshipId in 
	(select Id from Payments2.Apprenticeship where Ukprn = {0})

delete from Payments2.ApprenticeshipPause where ApprenticeshipId in 
	(select Id from Payments2.Apprenticeship where Ukprn = {0})

delete from Payments2.ApprenticeshipDuplicate where ApprenticeshipId in
	(select Id from Payments2.Apprenticeship where Ukprn = {0} )

delete from Payments2.DataLockEventNonPayablePeriodFailures where ApprenticeshipId in
	(select Id from Payments2.Apprenticeship where Ukprn = {0} )

delete from Payments2.Apprenticeship where Ukprn = {0}

delete from Payments2.DataLockEventNonPayablePeriod where DataLockEventId in 
	(select EventId from Payments2.DataLockEvent where Ukprn = {0})

delete from Payments2.DataLockEventPayablePeriod where DataLockEventId in 
	(select EventId from Payments2.DataLockEvent where Ukprn = {0})

delete from Payments2.DataLockEventPriceEpisode where DataLockEventId in 
	(select EventId from Payments2.DataLockEvent where Ukprn = {0})

delete from Payments2.DataLockFailure where Ukprn = {0}

delete from Payments2.DataLockEvent where Ukprn = {0}

delete from Payments2.EarningEventPeriod where EarningEventId in 
	(select EventId from Payments2.EarningEvent where Ukprn = {0})

delete from Payments2.EarningEventPriceEpisode where EarningEventId in 
	(select EventId from Payments2.EarningEvent where Ukprn = {0})

delete from Payments2.EarningEvent where Ukprn = {0}

delete from Payments2.EmployerProviderPriority where Ukprn = {0}

delete from Payments2.FundingSourceEvent where Ukprn = {0}

delete from Payments2.RequiredPaymentEvent where Ukprn = {0}

delete from Payments2.Payment where Ukprn = {0}

delete from Payments2.SubmittedLearnerAim where Ukprn = {0}
";

    }
}