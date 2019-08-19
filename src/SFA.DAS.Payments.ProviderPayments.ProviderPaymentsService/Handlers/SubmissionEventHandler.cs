using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public abstract class SubmissionEventHandler
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IHandleIlrSubmissionService submissionService;

        protected SubmissionEventHandler(IPaymentLogger paymentLogger,
            IHandleIlrSubmissionService submissionService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.submissionService = submissionService;
        }

        public async Task Handle(SubmissionEvent message, IMessageHandlerContext context)
        {
            var messageType = message.GetType().Name;

            try
            {
                paymentLogger.LogDebug($"Processing {messageType} for Message Id : {context.MessageId}");

                await HandleSubmission(submissionService, message);
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling {messageType}. Error: {ex.Message}, Job: {message.JobId}, UKPRN: {message.Ukprn}", ex);
                throw;
            }

            paymentLogger.LogDebug($"Finished processing {messageType} for Message Id : {context.MessageId}. ");
        }

        protected virtual Task HandleSubmission(IHandleIlrSubmissionService service,
            SubmissionEvent submissionEvent)
        {
            return Task.CompletedTask;
        }
    }
}