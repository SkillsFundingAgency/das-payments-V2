using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public interface ICurrentPriceEpisodeForJobStore
    {
        Task Add(CurrentPriceEpisode priceEpisode);
        Task<List<CurrentPriceEpisode>> GetCurrentPriceEpisodes(long ukprn);
        Task Replace(long jobId, long ukprn, IEnumerable<CurrentPriceEpisode> replacement);
    }

    public class CurrentPriceEpisodeForJobStore : ICurrentPriceEpisodeForJobStore
    {
        private readonly IPaymentsDataContext paymentsDataContext;

        public CurrentPriceEpisodeForJobStore(IPaymentsDataContext paymentsDataContext)
        {
            this.paymentsDataContext = paymentsDataContext;
        }

        public async Task Add(CurrentPriceEpisode priceEpisode)
        {
            await paymentsDataContext.CurrentPriceEpisodes.AddAsync(priceEpisode);
            await paymentsDataContext.SaveChangesAsync();
        }

        public async Task<List<CurrentPriceEpisode>> GetCurrentPriceEpisodes(long ukprn)
        {
            return await paymentsDataContext.CurrentPriceEpisodes
                .Where(x => x.Ukprn == ukprn)
                .ToListAsync();
        }

        public async Task Replace(long jobId, long ukprn, IEnumerable<CurrentPriceEpisode> priceEpisodes)
        {
            paymentsDataContext.CurrentPriceEpisodes
                .RemoveRange(paymentsDataContext
                    .CurrentPriceEpisodes
                    .Where(x => x.JobId == jobId && x.Ukprn == ukprn));

            await paymentsDataContext.CurrentPriceEpisodes.AddRangeAsync(priceEpisodes);
            await paymentsDataContext.SaveChangesAsync();

        }
    }
}