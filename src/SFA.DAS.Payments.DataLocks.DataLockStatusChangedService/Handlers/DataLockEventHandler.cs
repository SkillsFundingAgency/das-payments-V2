using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockStatusChangedService.Handlers
{
    public class DataLockEventHandler : IHandleMessages<DataLockEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IDataLockEventProcessor dataLockEventProcessor;

        public DataLockEventHandler(IPaymentLogger paymentLogger, IDataLockEventProcessor dataLockEventProcessor)
        {
            this.paymentLogger = paymentLogger;
            this.dataLockEventProcessor = dataLockEventProcessor;
        }

        public async Task Handle(DataLockEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogVerbose($"Processing {message.GetType().Name} event for UKPRN {message.Ukprn}");

            var dataLockStatusChangeMessages = await dataLockEventProcessor.ProcessDataLockEvent(message).ConfigureAwait(false);
            await Task.WhenAll(dataLockStatusChangeMessages.Select(context.Publish)).ConfigureAwait(false);

            paymentLogger.LogDebug($"Processed {message.GetType().Name} event for UKPRN {message.Ukprn}, generated {dataLockStatusChangeMessages.Count} events");
        }
    }
}
