using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.EarningEventsService.Handlers
{
    public class EarningEventHandler: IHandleMessages<EarningEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningEventProcessor processor;

        public EarningEventHandler(IPaymentLogger logger, IEarningEventProcessor processor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public async Task Handle(EarningEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Adding earning event to audit cache. Payment: {message.ToDebug()}");
            await processor.ProcessPaymentsEvent(message, CancellationToken.None);
        }
    }
}