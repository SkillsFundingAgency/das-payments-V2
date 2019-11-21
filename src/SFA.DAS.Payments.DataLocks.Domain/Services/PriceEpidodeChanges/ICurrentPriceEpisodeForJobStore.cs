using SFA.DAS.Payments.DataLocks.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges
{
    public interface ICurrentPriceEpisodeForJobStore
    {
        Task Add(CurrentPriceEpisode priceEpisode);
        Task<IEnumerable<CurrentPriceEpisode>> GetCurrentPriceEpisodes(long jobId, long ukprn);
        Task Replace(long jobId, long ukprn, IEnumerable<CurrentPriceEpisode> replacement);
    }


}
