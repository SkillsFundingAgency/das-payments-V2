using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.Audit.DataLockService.Handlers
{
    public class DataLockEventHandler: IHandleMessages<DataLockEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IDataLockEventProcessor processor;

        public DataLockEventHandler(IPaymentLogger logger, IDataLockEventProcessor processor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public async Task Handle(DataLockEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Adding data lock event to audit cache. Event: {message.ToDebug()}");
            await processor.ProcessPaymentsEvent(message, CancellationToken.None);
        }
    }
}