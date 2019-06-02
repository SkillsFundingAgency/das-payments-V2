using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Infrastructure;
using SFA.DAS.Payments.DataLocks.Domain.Services;

namespace SFA.DAS.Payments.DataLocks.Application.Cache
{
    public class DataLockLearnerCache : IDataLockLearnerCache
    {
        private readonly IActorDataCache<List<ApprenticeshipModel>> dataCache;
        private readonly IDataCache<bool> providerHasApprenticeshipsCache;

        public DataLockLearnerCache(IActorDataCache<List<ApprenticeshipModel>> dataCache, IDataCache<bool> providerHasApprenticeshipsCache)
        {
            this.dataCache = dataCache;
            this.providerHasApprenticeshipsCache = providerHasApprenticeshipsCache;
        }

        public async Task<bool> HasLearnerRecords()
        {
            var hasApprenticeships =  await providerHasApprenticeshipsCache.TryGet(CacheKeys.ProviderHasApprenticeshipsCacheKey).ConfigureAwait(false);
            return hasApprenticeships.HasValue && hasApprenticeships.Value ;
        }

        public async Task<List<ApprenticeshipModel>> GetLearnerApprenticeships(long uln)
        {
            var apprenticeships = await GetApprenticeships(uln.ToString()).ConfigureAwait(false);
            return apprenticeships;
        }

        public async Task<List<ApprenticeshipModel>> GetDuplicateApprenticeships()
        {
            var apprenticeships = await GetApprenticeships(CacheKeys.DuplicateApprenticeshipsKey).ConfigureAwait(false);
            return apprenticeships;
        }

        private async Task<List<ApprenticeshipModel>> GetApprenticeships(string key)
        {
            var apprenticeshipsCacheValue = await dataCache.TryGet(key.ToString()).ConfigureAwait(false);
            var apprenticeships = apprenticeshipsCacheValue.HasValue ? apprenticeshipsCacheValue.Value : new List<ApprenticeshipModel>();
            return apprenticeships;
        }

    }
}
