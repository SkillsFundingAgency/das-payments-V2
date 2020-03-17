using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class ProcessMonthEndAct1CompletionPaymentCommandHandler : IHandleMessages<ProcessMonthEndAct1CompletionPaymentCommand>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly ICompletionPaymentService completionPaymentService;

        public ProcessMonthEndAct1CompletionPaymentCommandHandler(IPaymentLogger paymentLogger, ICompletionPaymentService completionPaymentService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.completionPaymentService = completionPaymentService ?? throw new ArgumentNullException(nameof(completionPaymentService));
        }

        public async Task Handle(ProcessMonthEndAct1CompletionPaymentCommand message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Month End Act1 Completion Payment Command for Message Id : {context.MessageId}");

            var paymentEvents = await completionPaymentService.GetAct1CompletionPaymentEvents(message);

            foreach (var paymentEvent in paymentEvents)
            {
                paymentLogger.LogDebug($"Processing Act1 Completion Payment Event. Collection: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");

                await context.Publish(paymentEvent);
            }

            paymentLogger.LogInfo($"Successfully Processed Month End Act1 Completion Payment Command for {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");
        }
    }
}