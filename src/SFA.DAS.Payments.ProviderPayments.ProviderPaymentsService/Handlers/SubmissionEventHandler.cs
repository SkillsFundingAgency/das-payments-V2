using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public abstract class SubmissionEventHandler<T> : IHandleMessageBatches<T>
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

        protected abstract Task HandleSubmission(T submissionEvent, IHandleIlrSubmissionService service);
        public async Task Handle(IList<T> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                var messageType = message.GetType().Name;
                var logText = $"{messageType} for UKPRN: {message.Ukprn}, Academic Year: {message.AcademicYear}, Collection Period: {message.CollectionPeriod}, Job Id: {message.JobId}";
                paymentLogger.LogDebug($"Processing submission event.  {logText}");
                cancellationToken.ThrowIfCancellationRequested();
                await HandleSubmission(message, submissionService);
                paymentLogger.LogInfo($"Finished processing submission event.  {logText}");
            }
        }
    }
}