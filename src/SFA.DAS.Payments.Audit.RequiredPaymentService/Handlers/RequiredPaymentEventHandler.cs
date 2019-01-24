using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.RequiredPaymentService.Handlers
{
    public class RequiredPaymentEventHandler: IHandleMessages<RequiredPaymentEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IRequiredPaymentEventProcessor processor;

        public RequiredPaymentEventHandler(IPaymentLogger logger, IRequiredPaymentEventProcessor processor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public async Task Handle(RequiredPaymentEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Adding required payment to audit cache. Payment: {message.ToDebug()}");
            await processor.ProcessPaymentsEvent(message);
        }
    }
}