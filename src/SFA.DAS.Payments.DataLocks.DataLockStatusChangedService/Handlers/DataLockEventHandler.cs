using System.Collections.Generic;
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

            var dataLockStatusChangeMessages = new List<DataLockStatusChanged>();

            switch (message)
            {
                case PayableEarningEvent _:
                    dataLockStatusChangeMessages = await dataLockEventProcessor.ProcessPayableEarning(message).ConfigureAwait(false);
                    break;
                case EarningFailedDataLockMatching _:
                    dataLockStatusChangeMessages = await dataLockEventProcessor.ProcessDataLockFailure(message).ConfigureAwait(false);
                    break;
            }
            
            await Task.WhenAll(dataLockStatusChangeMessages.Select(context.Publish)).ConfigureAwait(false);

            paymentLogger.LogDebug($"Processed {message.GetType().Name} event for UKPRN {message.Ukprn}, generated {dataLockStatusChangeMessages.Count} events");
        }
    }
}
