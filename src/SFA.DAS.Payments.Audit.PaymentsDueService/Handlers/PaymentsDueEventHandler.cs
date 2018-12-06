using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.PaymentsDue;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.Audit.PaymentsDueService.Handlers
{
    public class PaymentsDueEventHandler: IHandleMessages<PaymentDueEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IPaymentsDueEventProcessor processor;

        public PaymentsDueEventHandler(IPaymentLogger logger, IPaymentsDueEventProcessor processor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public async Task Handle(PaymentDueEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Adding due payment to audit cache. Payment: {message.ToDebug()}");
            await processor.ProcessPaymentsEvent(message);
        }
    }
}