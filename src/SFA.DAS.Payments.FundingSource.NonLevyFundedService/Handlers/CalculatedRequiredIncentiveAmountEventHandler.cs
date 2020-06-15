using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.NonLevyFundedService.Handlers
{
    public class CalculatedRequiredIncentiveAmountEventHandler : IHandleMessages<CalculatedRequiredIncentiveAmount>
    {
        private readonly IPaymentLogger logger;
        private readonly IExecutionContext executionContext;
        private readonly IIncentiveRequiredPaymentProcessor incentiveProcessor;

        public CalculatedRequiredIncentiveAmountEventHandler(IPaymentLogger logger,
            IExecutionContext executionContext, 
            IIncentiveRequiredPaymentProcessor incentiveProcessor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.incentiveProcessor = incentiveProcessor ?? throw new ArgumentNullException(nameof(incentiveProcessor));
        }

        public async Task Handle(CalculatedRequiredIncentiveAmount message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Now processing the incentive required payment event. Learner ref: {message.Learner.ReferenceNumber}, Job id: {message.JobId}, Amount: {message.AmountDue}, Incentive type: {message.Type}.");
            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();
            var sfaFullyFundedPayment = incentiveProcessor.Process(message);
            await context.Publish(sfaFullyFundedPayment);
            logger.LogInfo($"Finished processing the incentive required payment event. Learner ref: {message.Learner.ReferenceNumber}, Job id: {message.JobId}, Amount: {message.AmountDue}, Incentive type: {message.Type}.");
        }
    }
}