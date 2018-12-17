using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.JobStatus.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Handlers
{
    public class ProcessedPaymentsMessageEventHandler: IHandleMessages<ProcessedPaymentsMessageEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IProviderEarningsJobService providerEarningsService;

        public ProcessedPaymentsMessageEventHandler(IPaymentLogger logger, IProviderEarningsJobService providerEarningsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.providerEarningsService = providerEarningsService ?? throw new ArgumentNullException(nameof(providerEarningsService));
        }

        public async Task Handle(ProcessedPaymentsMessageEvent message, IMessageHandlerContext context)
        {
            logger.LogVerbose($"Handling processed provider payment message. DC Job Id: {message.JobId}");
            await providerEarningsService.JobStepCompleted(message);
            logger.LogDebug($"Finished handling processed provider payment message. DC Job Id: {message.JobId}");
        }
    }
}