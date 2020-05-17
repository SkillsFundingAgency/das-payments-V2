using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.ServiceFabric.Core.Messaging
{
    public class DuplicateEarningEventService : IDuplicateEarningEventService
    {
        private readonly IPaymentLogger logger;
        private readonly IActorDataCache<EarningEventKey> cache;

        public DuplicateEarningEventService(IPaymentLogger logger, IActorDataCache<EarningEventKey> cache)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<bool> IsDuplicate(PaymentsEvent earningEvent, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Checking if earning event of type {earningEvent.GetType().Name} with guid: {earningEvent.EventId} has already been received.");
            var earningEventKey = new EarningEventKey(earningEvent);

            logger.LogDebug($"Earning event key: {earningEventKey.LogSafeKey}");
            if (await cache.Contains(earningEventKey.Key, cancellationToken).ConfigureAwait(false))
            {
                logger.LogWarning($"Key: {earningEventKey.LogSafeKey} found in the cache and is probably a duplicate.");
                return true;
            }
            logger.LogDebug($"New earnings event. Event key: {earningEventKey.LogSafeKey}, event id: {earningEvent.EventId}");
            await cache.Add(earningEventKey.Key, earningEventKey, cancellationToken);
            logger.LogInfo($"Added new earnings event to cache. Key: {earningEventKey.LogSafeKey}");
            return false;
        }
    }
}