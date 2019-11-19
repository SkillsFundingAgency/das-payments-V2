using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
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

        public Task<IEnumerable<CurrentPriceEpisode>> GetCurentPriceEpisodes(long jobId, long ukprn)
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
