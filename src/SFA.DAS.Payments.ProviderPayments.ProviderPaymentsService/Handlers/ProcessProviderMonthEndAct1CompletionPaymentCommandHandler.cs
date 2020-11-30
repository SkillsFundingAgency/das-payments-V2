using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Infrastructure;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class ProcessProviderMonthEndAct1CompletionPaymentCommandHandler : IHandleMessages<ProcessProviderMonthEndAct1CompletionPaymentCommand>
    {
        private readonly IDasEndpointFactory dasEndpointFactory;
        private readonly IPaymentLogger paymentLogger;
        private readonly ICompletionPaymentService completionPaymentService;

        public ProcessProviderMonthEndAct1CompletionPaymentCommandHandler(IDasEndpointFactory dasEndpointFactory, IPaymentLogger paymentLogger, ICompletionPaymentService completionPaymentService)
        {
            this.dasEndpointFactory = dasEndpointFactory ?? throw new ArgumentNullException(nameof(dasEndpointFactory));
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.completionPaymentService = completionPaymentService ?? throw new ArgumentNullException(nameof(completionPaymentService));
        }

        public async Task Handle(ProcessProviderMonthEndAct1CompletionPaymentCommand message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Provider Month End Act1 Completion Payment Command with ukprn: {message.Ukprn}, Message Id : {context.MessageId}");

            var paymentEvents = await completionPaymentService.GetAct1CompletionPaymentEvents(message);

            if (!paymentEvents.Any())
            {
                paymentLogger.LogWarning($"No Act1 Completion Payment Event Found for ukprn: {message.Ukprn}, Collection: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");

                return;
            }

            var dasEndPoint = await dasEndpointFactory.GetEndpointInstanceAsync();

            foreach (var paymentEvent in paymentEvents)
            {
                paymentLogger.LogDebug($"Processing Act1 Completion Payment Event. Ukprn: {message.Ukprn}, Collection: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");

                await dasEndPoint.Publish(paymentEvent);
            }

            paymentLogger.LogInfo($"Successfully Processed Month End Act1 Completion Payment Command for  ukprn: {message.Ukprn}, Collection:{message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");
        }
    }
}