using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Services;

namespace SFA.DAS.Payments.DataLocks.Application.Cache
{
    public class DataLockLearnerCache : IDataLockLearnerCache
    {
        private readonly IActorDataCache<List<ApprenticeshipModel>> dataCache;

        public DataLockLearnerCache(IActorDataCache<List<ApprenticeshipModel>> dataCache)
        {
            this.dataCache = dataCache;
        }

        public async Task<bool> HasLearnerRecords()
        {
            var isEmpty = await dataCache.IsEmpty().ConfigureAwait(false);
            return !isEmpty;
        }

        public async Task<List<ApprenticeshipModel>> GetLearnerApprenticeships(long uln)
        {
            var apprenticeshipsCacheValue = await dataCache.TryGet(uln.ToString()).ConfigureAwait(false);
            var apprenticeships = apprenticeshipsCacheValue.HasValue ? apprenticeshipsCacheValue.Value : new List<ApprenticeshipModel>();
            return apprenticeships;
        }

    }
}
