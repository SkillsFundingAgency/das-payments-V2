using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockEventService.Handlers
{
    public class PriceEpisodeStatusChangeEventHandler : IHandleMessages<PriceEpisodeStatusChange>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly ICachingEventProcessor<PriceEpisodeStatusChange> eventProcessor;

        public PriceEpisodeStatusChangeEventHandler(IPaymentLogger paymentLogger, ICachingEventProcessor<PriceEpisodeStatusChange> eventProcessor)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
        }

        public Task Handle(PriceEpisodeStatusChange message, IMessageHandlerContext context)
        {
            paymentLogger.LogDebug($"Processing {message.GetType().Name} event for UKPRN {message.DataLock.UKPRN}");

            return eventProcessor.EnqueueEvent(message, CancellationToken.None);
        }
    }
}
