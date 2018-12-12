using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class ProcessProviderMonthEndCommandHandler : IHandleMessages<ProcessProviderMonthEndCommand>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IExecutionContext executionContext;
        private readonly IMonthEndService monthEndService;
        private readonly IMapper mapper;
        private readonly IProviderPaymentFactory paymentFactory;


        public ProcessProviderMonthEndCommandHandler(IPaymentLogger paymentLogger,
            IExecutionContext executionContext,
            IMonthEndService monthEndService,
            IMapper mapper,
            IProviderPaymentFactory paymentFactory)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.monthEndService = monthEndService ?? throw new ArgumentNullException(nameof(monthEndService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.paymentFactory = paymentFactory ?? throw new ArgumentNullException(nameof(paymentFactory));
        }

        public async Task Handle(ProcessProviderMonthEndCommand message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Provider Month End Command for Message Id: {context.MessageId}");
            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();
            try
            {
                var payments = await monthEndService.GetMonthEndPayments(message.CollectionPeriod.Year, message.CollectionPeriod.Month, message.Ukprn, CancellationToken.None);
                foreach (var paymentEvent in payments.Select(MapToProviderPaymentEvent))
                {
                    await context.Publish(paymentEvent);
                }
                paymentLogger.LogInfo($"Successfully processed Month End Command for Job Id {message.JobId} and Message Type {message.GetType().Name}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError("Error while processing Process Provider Month End Command", ex);
                throw;
            }
        }

        private ProviderPaymentEvent MapToProviderPaymentEvent(PaymentModel payment)
        {
            paymentLogger.LogVerbose($"Mapping payment id: {payment.Id}, funding source: {payment.FundingSource}");
            var providerPayment = paymentFactory.Create(payment.FundingSource);
            paymentLogger.LogVerbose($"Got {providerPayment.GetType().Name} payment message type. Now mapping to provider payment.");
            mapper.Map(payment, providerPayment);
            paymentLogger.LogVerbose($"Finished mapping payment. Id: {providerPayment.EventId}");
            return providerPayment;
        }
    }
}
