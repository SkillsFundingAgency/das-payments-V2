using AutoMapper;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class FundingSourcePaymentEventHandler : IHandleMessages<FundingSourcePaymentEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IProviderPaymentsService paymentsService;
        private readonly IMapper mapper;
        private readonly IProcessAfterMonthEndPaymentService afterMonthEndPaymentService;

        public FundingSourcePaymentEventHandler(
            IPaymentLogger paymentLogger,  
            IProviderPaymentsService paymentsService,
            IMapper mapper, 
            IProcessAfterMonthEndPaymentService afterMonthEndPaymentService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.paymentsService = paymentsService ?? throw new ArgumentNullException(nameof(paymentsService));
            this.mapper = mapper;
            this.afterMonthEndPaymentService = afterMonthEndPaymentService;
        }

        public async Task Handle(FundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Funding Source Payment Event for Message Id : {context.MessageId}");
            var paymentModel = mapper.Map<ProviderPaymentEventModel>(message);
            await paymentsService.ProcessPayment(paymentModel, default(CancellationToken)).ConfigureAwait(false);

            var providerPaymentEvents = await afterMonthEndPaymentService.GetPaymentEvent(message);
            if (providerPaymentEvents != null)
            {
                paymentLogger.LogInfo($"Sent {providerPaymentEvents.GetType().Name} for {message.JobId} and Message Type {message.GetType().Name}");
                paymentLogger.LogDebug($"Sending Provider Payment Event {JsonConvert.SerializeObject(providerPaymentEvents)} for Message Id : {context.MessageId}.  {message.ToDebug()}");
                await context.Publish(providerPaymentEvents).ConfigureAwait(false);
            }

            paymentLogger.LogInfo($"Finished processing Funding Source Payment Event for Message Id : {context.MessageId}.  {message.ToDebug()}");
        }
    }
}