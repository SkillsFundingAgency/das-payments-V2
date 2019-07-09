using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockEventService.Handlers
{
    public class DataLockStatusChangedEventHandler : IHandleMessages<DataLockStatusChanged>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly ICachingEventProcessor<DataLockStatusChanged> eventProcessor;

        public DataLockStatusChangedEventHandler(IPaymentLogger paymentLogger, ICachingEventProcessor<DataLockStatusChanged> eventProcessor)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
        }

        public async Task Handle(DataLockStatusChanged message, IMessageHandlerContext context)
        {
            paymentLogger.LogDebug($"Processing {message.GetType().Name} event for UKPRN {message.Ukprn}");

            try
            {
                await eventProcessor.EnqueueEvent(message, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error processing {message.GetType().Name} event for UKPRN {message.Ukprn}", ex);
            }
        }
    }
}
