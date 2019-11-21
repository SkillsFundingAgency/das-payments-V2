using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    public class CurrentPriceEpisodeContext : DbContext, ICurrentPriceEpisodeForJobStore
    {
        public DbSet<CurrentPriceEpisode> Prices { get; set; }

        public CurrentPriceEpisodeContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrentPriceEpisode>().Property<long>("Id");
        }

        public async Task Add(CurrentPriceEpisode priceEpisode)
        {
            await Prices.AddAsync(priceEpisode);
            await SaveChangesAsync();
        }

        public Task<IEnumerable<CurrentPriceEpisode>> GetCurrentPriceEpisodes(long jobId, long ukprn)
        {
            return Task.FromResult(Prices.AsEnumerable());
        }

        public async Task Replace(long jobId, long ukprn, IEnumerable<CurrentPriceEpisode> priceEpisodes)
        {
            Prices.RemoveRange(Prices.Where(x => x.JobId == jobId && x.Ukprn == ukprn));
            await Prices.AddRangeAsync(priceEpisodes);
            await SaveChangesAsync();

        }
    }
}
