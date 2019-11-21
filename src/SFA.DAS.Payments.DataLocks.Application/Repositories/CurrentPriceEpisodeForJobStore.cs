using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public class CurrentPriceEpisodeForJobStore : ICurrentPriceEpisodeForJobStore
    {
        public Task Add(CurrentPriceEpisode priceEpisode)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<CurrentPriceEpisode>> GetCurrentPriceEpisodes(long jobId, long ukprn)
        {
            throw new System.NotImplementedException();
        }

        public Task Replace(long jobId, long ukprn, IEnumerable<CurrentPriceEpisode> replacement)
        {
            throw new System.NotImplementedException();
        }
    }
}
