using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using ESFA.DC.JobStatus.Interface;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.TestModels;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    internal class UkprnService : DbContext, IUkprnService
    {
        private readonly IJobService jobService;

        public UkprnService(DbContextOptions<UkprnService> options, IJobService jobService) : base(options)
        {
            this.jobService = jobService;
        }

        public int GenerateUkprn()
        {
            string appGuid =
                ((GuidAttribute)Assembly.GetExecutingAssembly().
                    GetCustomAttributes(typeof(GuidAttribute), false).
                    GetValue(0)).Value.ToString();

            Provider provider = null;
            using (var mutex = new Mutex(false, $"Global\\{{{appGuid}}}"))
            {
                if (mutex.WaitOne(TimeSpan.FromMinutes(1)))
                {
                    provider = GetProvider();
                    // check job queue for ukprn - looking for status 2 or 3 which will block queue
                    var blockedList = jobService.GetJobsByStatus(provider.Ukprn, 2, 3).Result;
                    if (blockedList.Any())
                    {
                        provider = GetProvider();
                    }

                    ClearPaymentsData(provider);
                    mutex.ReleaseMutex();
                }
                else
                {
                    throw new ApplicationException("Unable to obtain a Ukprn due to a locked Mutex");
                }
            }

            return provider.Ukprn;
        }

        private Provider GetProvider()
        {
            Provider provider;
            provider = LeastRecentlyUsed();
            provider.Use();
            SaveChanges();
            return provider;
        }

        private Provider LeastRecentlyUsed() =>
            Providers.OrderBy(x => x.LastUsed).FirstOrDefault()
            ?? throw new InvalidOperationException("There are no UKPRNs available in the well-known Providers pool.");

        private void ClearPaymentsData(Provider provider)
        {
            const string DeleteUkprnData = @"
delete from Payments2.ApprenticeshipPriceEpisode where ApprenticeshipId in 
	(select Id from Payments2.Apprenticeship where Ukprn = {0})

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
