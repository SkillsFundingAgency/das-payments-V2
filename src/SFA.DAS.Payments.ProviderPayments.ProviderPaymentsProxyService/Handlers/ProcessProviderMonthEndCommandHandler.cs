using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsProxyService.Handlers
{
    public class ProcessProviderMonthEndCommandHandler : IHandleMessages<ProcessProviderMonthEndCommand>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IExecutionContext executionContext;
        private readonly IActorProxyFactory proxyFactory;
        private readonly IMapper mapper;
        private readonly IProviderPaymentFactory paymentFactory;


        public ProcessProviderMonthEndCommandHandler(IPaymentLogger paymentLogger,
            IExecutionContext executionContext,
            IActorProxyFactory proxyFactory,
            IMapper mapper,
            IProviderPaymentFactory paymentFactory)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.paymentFactory = paymentFactory ?? throw new ArgumentNullException(nameof(paymentFactory));
        }


        public async Task Handle(ProcessProviderMonthEndCommand message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Provider Month End Command for Message Id : {context.MessageId}");
            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();

            try
            {
                var actorId = new ActorId(message.Ukprn.ToString());
                var actor = proxyFactory.CreateActorProxy<ProviderPaymentsService.Interfaces.IProviderPaymentsService>(new Uri("fabric:/SFA.DAS.Payments.ProviderPayments.ServiceFabric/ProviderPaymentsServiceActorService"), actorId);
                var payments = await actor.HandleMonthEnd(message.CollectionPeriod.Year, message.CollectionPeriod.Month, new CancellationToken());
                foreach (var paymentEvent in payments.Select(MapToProviderPaymentEvent))
                {
                    await context.Publish(paymentEvent);
                }
                paymentLogger.LogInfo($"Successfully processed Month End Command for Job Id {message.JobId} and Message Type {message.GetType().Name}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while processing Process Provider Month End Command", ex);
                throw;
            }
        }

        private ProviderPaymentEvent MapToProviderPaymentEvent(PaymentModel payment)
        {
            paymentLogger.LogDebug($"Mapping payment id: {payment.Id}, funding source: {payment.FundingSource}");
            var providerPayment = paymentFactory.Create(payment.FundingSource);
            paymentLogger.LogDebug($"Got {providerPayment.GetType().Name} payment message type. Now mapping to provider payment.");
            mapper.Map(payment, providerPayment);
            paymentLogger.LogDebug($"Finished mapping payment. Id: {providerPayment.ExternalId}");
            return providerPayment;
        }
    }
}
