using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Domain.Infrastructure;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using IMapper = AutoMapper.IMapper;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public interface IApprenticeshipUpdatedProcessor
    {
        Task ProcessApprenticeshipUpdate(ApprenticeshipUpdated updatedApprenticeship);
        Task<List<DataLockEvent>> GetApprenticeshipUpdatePayments(ApprenticeshipUpdated updatedApprenticeship);
        Task<List<FunctionalSkillDataLockEvent>> GetApprenticeshipUpdateFunctionalSkillPayments(ApprenticeshipUpdated updatedApprenticeship);
    }

    public class ApprenticeshipUpdatedProcessor : IApprenticeshipUpdatedProcessor
    {
        private readonly IPaymentLogger logger;
        private readonly IActorDataCache<List<ApprenticeshipModel>> dataCache;
        private readonly IMapper mapper;
        private readonly IActorDataCache<List<long>> ukprnCache;

        public ApprenticeshipUpdatedProcessor(IPaymentLogger logger,
            IActorDataCache<List<ApprenticeshipModel>> dataCache,
            IMapper mapper,
            IActorDataCache<List<long>> ukprnCache)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataCache = dataCache ?? throw new ArgumentNullException(nameof(dataCache));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.ukprnCache = ukprnCache ?? throw new ArgumentNullException(nameof(ukprnCache));
        }

        public async Task ProcessApprenticeshipUpdate(ApprenticeshipUpdated updatedApprenticeship)
        {
            logger.LogDebug(
                $"Getting apprenticeships cache item using uln for apprenticeship id: {updatedApprenticeship.Id}");
            var cacheItem = await dataCache.TryGet(updatedApprenticeship.Uln.ToString(), CancellationToken.None);
            logger.LogDebug(cacheItem.HasValue
                ? "Item found in the cache."
                : "No cache item found. Will now create new apprenticeships list.");
            var apprenticeships = cacheItem.HasValue ? cacheItem.Value : new List<ApprenticeshipModel>();
            logger.LogDebug("Removing old version of the apprenticeship.");
            apprenticeships.RemoveAll(apprenticeship => apprenticeship.Id == updatedApprenticeship.Id);
            logger.LogDebug("Now mapping the ApprenticeshipUpdated event to the ApprenticeshipModel.");
            var model = mapper.Map<ApprenticeshipModel>(updatedApprenticeship);
            logger.LogDebug("Finished mapping the apprenticeship model, now adding to the cache.");
            apprenticeships.Add(model);
            await dataCache.AddOrReplace(model.Uln.ToString(), apprenticeships, CancellationToken.None);
            logger.LogInfo(
                $"Finished storing the apprenticeship details in the cache. Apprenticeship id: {model.Id}, Account: {model.AccountId}, Provider: {model.Ukprn}");
            await AddUkprnToProviderCache(updatedApprenticeship.Ukprn);
        }

        private async Task AddUkprnToProviderCache(long ukprn)
        {
            var cacheItem = await ukprnCache.TryGet(CacheKeys.ProvidersKey, CancellationToken.None);
            var ukprnList = cacheItem.HasValue ? cacheItem.Value : new List<long>();
            if (!ukprnList.Contains(ukprn))
            {
                ukprnList.Add(ukprn);
            }
            await ukprnCache.AddOrReplace(CacheKeys.ProvidersKey, ukprnList, CancellationToken.None);
        }

        public async Task<List<DataLockEvent>> GetApprenticeshipUpdatePayments(ApprenticeshipUpdated updatedApprenticeship)
        {
            return null;
        }
        
        public async Task<List<FunctionalSkillDataLockEvent>> GetApprenticeshipUpdateFunctionalSkillPayments(ApprenticeshipUpdated updatedApprenticeship)
        {
            return null;
        }
    }
}