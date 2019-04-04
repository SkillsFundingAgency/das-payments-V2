using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.Model.Core.Entities;

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
    }
}
