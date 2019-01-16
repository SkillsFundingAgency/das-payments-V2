using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class IlrSubmittedEventHandler : IHandleMessages<IlrSubmittedEvent>
    {

        private readonly IPaymentLogger logger;
        private readonly IHandleIlrSubmissionService handleIlrSubmissionService;
        private readonly IExecutionContext executionContext;

        public IlrSubmittedEventHandler(IPaymentLogger logger,
            IHandleIlrSubmissionService handleIlrSubmissionService,
            IExecutionContext executionContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.handleIlrSubmissionService = handleIlrSubmissionService ?? throw new ArgumentNullException(nameof(handleIlrSubmissionService));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
        }

        public async Task Handle(IlrSubmittedEvent message, IMessageHandlerContext context)
        {
            try
            {
                var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
                currentExecutionContext.JobId = message.JobId.ToString();
                logger.LogDebug(
                    $"Processing Ilr Submitted Event for Message: {message.CollectionPeriod.Name}, Ukprn: {message.Ukprn}, Job id: {message.JobId}, Ilr submission time: {message.IlrSubmissionDateTime}");
                await handleIlrSubmissionService.Handle(message, CancellationToken.None);
                logger.LogInfo(
                    $"Successfully processed Ilr Submitted Event for Job Id {message.JobId} and Message Type {message.GetType().Name}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling the ILR submitted event. Ex: {ex.Message}", ex);
                throw;
            }
        }
    }
}
