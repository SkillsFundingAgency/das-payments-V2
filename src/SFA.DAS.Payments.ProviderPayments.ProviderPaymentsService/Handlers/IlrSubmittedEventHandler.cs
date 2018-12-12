using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class IlrSubmittedEventHandler : IHandleMessages<IlrSubmittedEvent>
    {

        private readonly IPaymentLogger paymentLogger;
        private readonly IHandleIlrSubmissionService handleIlrSubmissionService;
        private readonly IExecutionContext executionContext;

        public IlrSubmittedEventHandler(IPaymentLogger paymentLogger,
            IHandleIlrSubmissionService handleIlrSubmissionService,
            IExecutionContext executionContext)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.handleIlrSubmissionService = handleIlrSubmissionService ?? throw new ArgumentNullException(nameof(handleIlrSubmissionService));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
        }
        
        public async Task Handle(IlrSubmittedEvent message, IMessageHandlerContext context)
        {
            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();
            paymentLogger.LogDebug($"Processing Ilr Submitted Event for Message: {message.CollectionPeriod.Name}, Ukprn: {message.Ukprn}, Job id: {message.JobId}, Ilr submission time: {message.IlrSubmissionDateTime}");
            await handleIlrSubmissionService.Handle(message, CancellationToken.None);
            paymentLogger.LogInfo($"Successfully processed Ilr Submitted Event for Job Id {message.JobId} and Message Type {message.GetType().Name}");
        }
    }
}
