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
    public class ProcessMonthEndAct1CompletionPaymentCommandHandler : IHandleMessages<ProcessMonthEndAct1CompletionPaymentCommand>
    {
        private readonly IDasEndpointFactory dasEndpointFactory;
        private readonly IPaymentLogger paymentLogger;
        private readonly ICompletionPaymentService completionPaymentService;

        public ProcessMonthEndAct1CompletionPaymentCommandHandler(IDasEndpointFactory dasEndpointFactory, IPaymentLogger paymentLogger, ICompletionPaymentService completionPaymentService)
        {
            this.dasEndpointFactory = dasEndpointFactory ?? throw new ArgumentNullException(nameof(dasEndpointFactory));
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.completionPaymentService = completionPaymentService ?? throw new ArgumentNullException(nameof(completionPaymentService));
        }

        public async Task Handle(ProcessMonthEndAct1CompletionPaymentCommand message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Month End Act1 Completion Payment Command for Message Id : {context.MessageId}");

            var paymentEvents = await completionPaymentService.GetAct1CompletionPaymentEvents(message);

            if (!paymentEvents.Any())
            {
                paymentLogger.LogDebug($"No Act1 Completion Payment Event Found for Collection: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");

                return;
            }

            var dasEndPoint = await dasEndpointFactory.GetEndpointInstanceAsync();

            foreach (var paymentEvent in paymentEvents)
            {
                paymentLogger.LogDebug($"Processing Act1 Completion Payment Event. Collection: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");

                await dasEndPoint.Publish(paymentEvent);
            }

            paymentLogger.LogInfo($"Successfully Processed Month End Act1 Completion Payment Command for {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");
        }
    }
}