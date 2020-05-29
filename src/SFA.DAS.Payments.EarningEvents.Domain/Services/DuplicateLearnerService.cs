using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Domain.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Domain.Services
{
    public class DuplicateLearnerService : IDuplicateLearnerService
    {
        private readonly IPaymentLogger logger;
        private readonly IActorDataCache<LearnerKey> cache;

        public DuplicateLearnerService(IPaymentLogger logger, IActorDataCache<LearnerKey> cache)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<bool> IsDuplicate(ProcessLearnerCommand processLearnerCommand, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Checking if command of type {processLearnerCommand.GetType().Name} with guid: {processLearnerCommand.CommandId} has already been received.");

            var learnerKey = new LearnerKey(processLearnerCommand);

            logger.LogDebug($"learner key: {learnerKey.LogSafeKey}");
            if (await cache.Contains(learnerKey.Key, cancellationToken).ConfigureAwait(false))
            {
                logger.LogWarning($"Key: {learnerKey.LogSafeKey} found in the cache and is probably a duplicate.");
                return true;
            }
            logger.LogDebug($"New learner command. Command key: {learnerKey.LogSafeKey}, command id: {processLearnerCommand.CommandId}");
            await cache.Add(learnerKey.Key, learnerKey, cancellationToken);
            logger.LogInfo($"Added new earnings event to cache. Key: {learnerKey.LogSafeKey}");
            return false;
        }
    }
}