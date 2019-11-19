using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using System.Collections.Generic;

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

        public void Replace(long jobId, long ukprn, IEnumerable<CurrentPriceEpisode> replacement)
        {
            throw new System.NotImplementedException();
        }
    }
}
