using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class FundingSourcePaymentEventHandler : IHandleMessages<FundingSourcePaymentEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly Application.Services.IProviderPaymentsService paymentsService;
        private readonly IMapper mapper;

        public FundingSourcePaymentEventHandler(IPaymentLogger paymentLogger,
            Application.Services.IProviderPaymentsService paymentsService, IMapper mapper)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.paymentsService = paymentsService ?? throw new ArgumentNullException(nameof(paymentsService));
            this.mapper = mapper;
        }

        public async Task Handle(FundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogDebug($"Processing Funding Source Payment Event for Message Id : {context.MessageId}");
            var paymentModel = mapper.Map<ProviderPaymentEventModel>(message);
            await paymentsService.ProcessPayment(paymentModel, default(CancellationToken));
            paymentLogger.LogDebug($"finished processing Funding Source Payment Event for Message Id : {context.MessageId}.  {message.ToDebug()}");
        }
    }
}
