using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.ServiceFabric.Core.Messaging
{
    public class DuplicatePeriodisedPaymentEventService : IDuplicatePeriodisedPaymentEventService
    {
        private readonly IPaymentLogger logger;
        private readonly IActorDataCache<PeriodisedPaymentEventKey> cache;

        public DuplicatePeriodisedPaymentEventService(IPaymentLogger logger, IActorDataCache<PeriodisedPaymentEventKey> cache)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<bool> IsDuplicate(IPeriodisedPaymentEvent periodisedPaymentEvent, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Checking if periodised payment event of type {periodisedPaymentEvent.GetType().Name} with guid: {periodisedPaymentEvent.EventId} has already been received.");
            var periodisedPaymentEventKey = new PeriodisedPaymentEventKey(periodisedPaymentEvent);

            logger.LogDebug($"Earning event key: {periodisedPaymentEventKey.LogSafeKey}");
            if (await cache.Contains(periodisedPaymentEventKey.Key, cancellationToken).ConfigureAwait(false))
            {
                logger.LogWarning($"Key: {periodisedPaymentEventKey.LogSafeKey} found in the cache and is probably a duplicate.");
                return true;
            }
            logger.LogDebug($"New periodised payment event. Event key: {periodisedPaymentEventKey.LogSafeKey}, event id: {periodisedPaymentEvent.EventId}");
            await cache.Add(periodisedPaymentEventKey.Key, periodisedPaymentEventKey, cancellationToken);
            logger.LogInfo($"Added new periodised payment event to cache. Key: {periodisedPaymentEventKey.LogSafeKey}");
            return false;
        }
    }
}