using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public class CurrentPriceEpisodeForJobStore : ICurrentPriceEpisodeForJobStore
    {
        public void Add(CurrentPriceEpisode priceEpisode)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CurrentPriceEpisode> GetCurentPriceEpisodes(long jobId, long ukprn)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(long jobId, long ukprn)
        {
            throw new System.NotImplementedException();
        }
    }
}
