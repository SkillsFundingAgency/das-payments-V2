using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using IMapper = AutoMapper.IMapper;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public interface IApprenticeshipUpdatedProcessor
    {
        Task ProcessApprenticeshipUpdate(ApprenticeshipUpdated updatedApprenticeship);
    }

    public class ApprenticeshipUpdatedProcessor : IApprenticeshipUpdatedProcessor
    {
        private readonly IPaymentLogger logger;
        private readonly IActorDataCache<List<ApprenticeshipModel>> cache;
        private readonly IMapper mapper;

        public ApprenticeshipUpdatedProcessor(IPaymentLogger logger, IActorDataCache<List<ApprenticeshipModel>> cache,
            IMapper mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task ProcessApprenticeshipUpdate(ApprenticeshipUpdated updatedApprenticeship)
        {
            logger.LogDebug(
                $"Getting apprenticeships cache item using uln for apprenticeship id: {updatedApprenticeship.Id}");
            var cacheItem = await cache.TryGet(updatedApprenticeship.Uln.ToString(), CancellationToken.None);
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
            await cache.AddOrReplace(model.Uln.ToString(), apprenticeships, CancellationToken.None);
            logger.LogInfo(
                $"Finished storing the apprenticeship details in the cache. Apprenticeship id: {model.Id}, Account: {model.AccountId}, Provider: {model.Ukprn}");
        }

        public async Task GetDataLockEarningsToReprocessed(ApprenticeshipUpdated updatedApprenticeship)
        {

            await Task.CompletedTask;
        }
    }
}