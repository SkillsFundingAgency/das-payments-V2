using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public interface IReceivedEarningEventService
    {
        Task<bool> AlreadyReceived(PaymentsEvent earningEvent, CancellationToken cancellationToken);
    }

    public class ReceivedEarningEventService : IReceivedEarningEventService
    {
        private readonly IPaymentLogger logger;
        private readonly IActorDataCache<PaymentsEvent> cache;

        public ReceivedEarningEventService(IPaymentLogger logger, IActorDataCache<PaymentsEvent> cache)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<bool> AlreadyReceived(PaymentsEvent earningEvent, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Checking if earning event of type {earningEvent.GetType().Name} with guid: {earningEvent.EventId} has already been received.");
            var key = CreateKey(earningEvent);
            var logSafeKey = SanitiseKey(earningEvent);
            logger.LogDebug($"Earning event key: {logSafeKey}");
            if (await cache.Contains(key, cancellationToken).ConfigureAwait(false))
            {
                logger.LogWarning($"Key: {logSafeKey} found in the cache and is probably a duplicate.");
                return true;
            }
            logger.LogDebug($"First time processing earning event. Event key: {logSafeKey}, event id: {earningEvent.EventId}");
            await cache.Add(key, earningEvent, cancellationToken);
            return false;
        }

        private string CreateKey(PaymentsEvent earningEvent)
        {
            return $@"{earningEvent.JobId}-{earningEvent.Ukprn}-{earningEvent.CollectionPeriod.AcademicYear}-{earningEvent.CollectionPeriod.Period}-
                        {earningEvent.Learner.Uln}-{earningEvent.Learner.ReferenceNumber}-{earningEvent.LearningAim.Reference}-
                        {earningEvent.LearningAim.ProgrammeType}-{earningEvent.LearningAim.StandardCode}-{earningEvent.LearningAim.FrameworkCode}-
                        {earningEvent.LearningAim.PathwayCode}-{earningEvent.LearningAim.FundingLineType}-{earningEvent.LearningAim.SequenceNumber}-
                        {earningEvent.LearningAim.StartDate:G}-{earningEvent.GetType().Name}";
        }

        private string SanitiseKey(PaymentsEvent earningEvent)
        {
            return $@"{earningEvent.JobId}-{earningEvent.CollectionPeriod.AcademicYear}-{earningEvent.CollectionPeriod.Period}-
                        {earningEvent.Learner.ReferenceNumber}-{earningEvent.LearningAim.Reference}-
                        {earningEvent.LearningAim.ProgrammeType}-{earningEvent.LearningAim.StandardCode}-{earningEvent.LearningAim.FrameworkCode}-
                        {earningEvent.LearningAim.PathwayCode}-{earningEvent.LearningAim.FundingLineType}-{earningEvent.LearningAim.SequenceNumber}-
                        {earningEvent.LearningAim.StartDate:G}-{earningEvent.GetType().Name}";
        }
    }
}