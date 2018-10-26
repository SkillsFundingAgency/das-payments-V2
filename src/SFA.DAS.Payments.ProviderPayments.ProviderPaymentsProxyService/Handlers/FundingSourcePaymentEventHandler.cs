using AutoMapper;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsProxyService.Handlers
{
    public class FundingSourcePaymentEventHandler : IHandleMessages<FundingSourcePaymentEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IExecutionContext executionContext;
        private readonly IActorProxyFactory proxyFactory;
        private readonly IMapper mapper;

        public FundingSourcePaymentEventHandler(IPaymentLogger paymentLogger,
            IExecutionContext executionContext,
            IActorProxyFactory proxyFactory,
            IMapper mapper)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.proxyFactory = proxyFactory;
            this.mapper = mapper;
        }

        public async Task Handle(FundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Funding Source Payment Event for Message Id : {context.MessageId}");

            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();

            try
            {
                var actorId = new ActorId(message.Ukprn.ToString());
                var actor = proxyFactory.CreateActorProxy<IProviderPaymentsService>(new Uri("fabric:/SFA.DAS.Payments.ProviderPayments.ServiceFabric/ProviderPaymentsServiceActorService"), actorId);

                var paymentModel = mapper.Map<PaymentModel>(message);

                await actor.ProcessPayment(paymentModel, new CancellationToken());
                paymentLogger.LogInfo($"Successfully processed Funding Source Payment Event for Job Id {message.JobId} and Message Type {message.GetType().Name}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling Provider PaymentsProxyService handler Event", ex);
                throw;
            }

            await Task.CompletedTask;
        }
    }
}
