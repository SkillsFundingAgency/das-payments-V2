using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class SubmissionSucceededEventHandler:IHandleMessages<SubmissionSucceededEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IHandleIlrSubmissionService submissionService;
        private readonly IMapper mapper;
        private readonly IEarningsJobClient earningsJobClient;
        private readonly IProcessAfterMonthEndPaymentService afterMonthEndPaymentService;

        public SubmissionSucceededEventHandler(IPaymentLogger paymentLogger,
            IHandleIlrSubmissionService submissionService,
            IMapper mapper,
            IEarningsJobClient earningsJobClient,
            IProcessAfterMonthEndPaymentService afterMonthEndPaymentService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.submissionService = submissionService;
            this.mapper = mapper;
            this.earningsJobClient = earningsJobClient;
            this.afterMonthEndPaymentService = afterMonthEndPaymentService;
        }

        public async Task Handle(SubmissionSucceededEvent message, IMessageHandlerContext context)
        {
            try
            {
                paymentLogger.LogDebug($"Processing Submission Succeeded Event for Message Id : {context.MessageId}");

                await submissionService.HandleSubmissionSucceeded(message, default(CancellationToken));
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling SubmissionSucceededEvent event. Error: {ex.Message}, Job: {message.JobId}, UKPRN: {message.Ukprn}", ex);
                throw;
            }

            paymentLogger.LogDebug($"Finished processing Submission Succeeded Event for Message Id : {context.MessageId}. ");
        }
    }

    public class SubmissionFailedEventHandler : IHandleMessages<SubmissionFailedEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IHandleIlrSubmissionService submissionService;
        private readonly IMapper mapper;
        private readonly IEarningsJobClient earningsJobClient;
        private readonly IProcessAfterMonthEndPaymentService afterMonthEndPaymentService;

        public SubmissionFailedEventHandler(IPaymentLogger paymentLogger,
            IHandleIlrSubmissionService submissionService,
            IMapper mapper,
            IEarningsJobClient earningsJobClient,
            IProcessAfterMonthEndPaymentService afterMonthEndPaymentService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.submissionService = submissionService;
            this.mapper = mapper;
            this.earningsJobClient = earningsJobClient;
            this.afterMonthEndPaymentService = afterMonthEndPaymentService;
        }

        public async Task Handle(SubmissionFailedEvent message, IMessageHandlerContext context)
        {
            try
            {
                paymentLogger.LogDebug($"Processing Submission Succeeded Event for Message Id : {context.MessageId}");

                await submissionService.HandleSubmissionFailed(message, default(CancellationToken));
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling SubmissionSucceededEvent event. Error: {ex.Message}, Job: {message.JobId}, UKPRN: {message.Ukprn}", ex);
                throw;
            }

            paymentLogger.LogDebug($"Finished processing Submission Succeeded Event for Message Id : {context.MessageId}. ");
        }
    }
}