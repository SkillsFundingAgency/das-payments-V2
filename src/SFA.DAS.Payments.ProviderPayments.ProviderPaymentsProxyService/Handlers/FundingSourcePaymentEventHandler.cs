using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsProxyService.Handlers
{
    public class FundingSourcePaymentEventHandler : IHandleMessages<FundingSourcePaymentEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IFundingSourceEventHandlerService fundingSourceEventHandlerService;
        private readonly IExecutionContext executionContext;

        public FundingSourcePaymentEventHandler(IPaymentLogger paymentLogger, IFundingSourceEventHandlerService fundingSourceEventHandlerService, IExecutionContext executionContext)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.fundingSourceEventHandlerService = fundingSourceEventHandlerService ?? throw new ArgumentNullException(nameof(fundingSourceEventHandlerService));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
        }

        public async Task Handle(FundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Funding Source Payment Event for Message Id : {context.MessageId}");

            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId;

            try
            {
                await fundingSourceEventHandlerService.ProcessEvent(message);

                paymentLogger.LogInfo($"Successfully processed Funding Source Payment Event for Job Id {message.JobId} and Message Type {message.GetType().Name}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling NonLevyFundedService event", ex);
                throw;
            }
        }
    }
}
