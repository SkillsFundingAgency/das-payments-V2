using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.FundingSourceService.Handlers
{
    public class FundingSourceEventHandler: IHandleMessages<FundingSourcePaymentEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IFundingSourceEventProcessor processor;

        public FundingSourceEventHandler(IPaymentLogger logger, IFundingSourceEventProcessor processor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public async Task Handle(FundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Adding funding source payment to audit cache. Payment: {message.ToDebug()}");
            await processor.ProcessPaymentsEvent(message, CancellationToken.None);
        }
    }
}