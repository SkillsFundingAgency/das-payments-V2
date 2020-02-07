using System;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Infrastructure;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.DataLocks.Application.Cache
{
    public class DataLockLearnerCache : IDataLockLearnerCache
    {
        private readonly IActorDataCache<List<ApprenticeshipModel>> dataCache;
        private readonly IActorDataCache<List<long>> ukprnCache;

        public DataLockLearnerCache(IActorDataCache<List<ApprenticeshipModel>> dataCache, IActorDataCache<List<long>> ukprnCache)
        {
            this.dataCache = dataCache ?? throw new ArgumentNullException(nameof(dataCache));
            this.ukprnCache = ukprnCache ?? throw new ArgumentNullException(nameof(ukprnCache));
        }

        public async Task<bool> UkprnExists(long ukprn)
        {
            var providersCacheItem = await ukprnCache.TryGet(CacheKeys.ProvidersKey).ConfigureAwait(false);
            return providersCacheItem.HasValue && providersCacheItem.Value.Contains(ukprn);
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

    public class DataLockLearnerDatabaseCache : IDataLockLearnerCache
    {
        private readonly Func<IApprenticeshipRepository> apprenticeshipRepository;
        private readonly IActorIdProvider actorIdProvider;
        private long learnerUln ;

        public DataLockLearnerDatabaseCache(Func<IApprenticeshipRepository> apprenticeshipRepository, IActorIdProvider actorIdProvider)
        {
            this.apprenticeshipRepository = apprenticeshipRepository ?? throw new ArgumentNullException(nameof(apprenticeshipRepository));
            this.actorIdProvider = actorIdProvider;
        }

        public async Task<bool> UkprnExists(long ukprn)
        {
            List<long> providerIds;
            learnerUln = actorIdProvider.Current.GetLongId();
            using (var repository = apprenticeshipRepository())
            {
                providerIds = await repository.GetProviderIdsByUln(learnerUln);
            }

            return providerIds != null && providerIds.Contains(ukprn);
        }

        public async Task<List<ApprenticeshipModel>> GetLearnerApprenticeships(long uln)
        {
            var apprenticeships = await GetApprenticeships(uln).ConfigureAwait(false);
            return apprenticeships;
        }

        public async Task<List<ApprenticeshipModel>> GetDuplicateApprenticeships()
        {
            learnerUln = actorIdProvider.Current.GetLongId();
            var apprenticeships = await GetApprenticeships(learnerUln).ConfigureAwait(false);
            return apprenticeships;
        }

        private async Task<List<ApprenticeshipModel>> GetApprenticeships(long uln)
        {
            List<ApprenticeshipModel> providerApprenticeships;

            using (var repository = apprenticeshipRepository())
            { 
                providerApprenticeships = await repository.ApprenticeshipsByUln(uln).ConfigureAwait(false);
            }

            var apprenticeships = providerApprenticeships?? new List<ApprenticeshipModel>();
            return apprenticeships;
        }

    }

}