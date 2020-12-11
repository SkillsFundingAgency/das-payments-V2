using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public abstract class SubmissionEventHandler<T> : IHandleMessages<T>
        where T: SubmissionJobFinishedEvent
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IHandleIlrSubmissionService submissionService;

        protected SubmissionEventHandler(IPaymentLogger paymentLogger,
            IHandleIlrSubmissionService submissionService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.submissionService = submissionService;
        }

        public async Task Handle(T message, IMessageHandlerContext context)
        {
            var messageType = message.GetType().Name;

            paymentLogger.LogDebug($"Processing {messageType} for Message Id : {context.MessageId}");

            await HandleSubmission(message, submissionService);

            paymentLogger.LogDebug($"Finished processing {messageType} for Message Id : {context.MessageId}. ");
        }

        protected abstract Task HandleSubmission(T submissionEvent, IHandleIlrSubmissionService service);
    }
}