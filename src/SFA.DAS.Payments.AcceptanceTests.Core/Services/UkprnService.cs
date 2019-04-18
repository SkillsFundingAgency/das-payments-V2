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
            //return 10036143;
            var provider = LeastRecentlyUsed();
            provider.Use();
            SaveChanges();
            return provider.Ukprn;
        }

        private Provider LeastRecentlyUsed() => 
            Providers.OrderByDescending(x => x.LastUsed).FirstOrDefault()
            ?? throw new InvalidOperationException("There are no UKPRNs available in the well-known Providers pool.");

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
