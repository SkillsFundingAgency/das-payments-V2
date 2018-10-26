using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsProxyService.Handlers
{
    public class IlrSubmittedEventHandler : IHandleMessages<IlrSubmittedEvent>
    {

        private readonly IPaymentLogger paymentLogger;
        private readonly IExecutionContext executionContext;
        private readonly IActorProxyFactory proxyFactory;

        public IlrSubmittedEventHandler(IPaymentLogger paymentLogger, 
            IExecutionContext executionContext,
            IActorProxyFactory proxyFactory)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.proxyFactory = proxyFactory;
        }
        
        public async Task Handle(IlrSubmittedEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Ilr Submitted Event for Message Id : {context.MessageId}");

            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();

            try
            {
                var actorId = new ActorId(message.Ukprn.ToString());
                var actor = proxyFactory.CreateActorProxy<ProviderPaymentsService.Interfaces.IProviderPaymentsService>(new Uri("fabric:/SFA.DAS.Payments.ProviderPayments.ServiceFabric/ProviderPaymentsServiceActorService"), actorId);

                await actor.HandleIlrSubMissionAsync(message, new CancellationToken());

                paymentLogger.LogInfo($"Successfully processed Ilr Submitted Event for Job Id {message.JobId} and Message Type {message.GetType().Name}");

            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling Provider Payments Ilr Submitted ProxyService Event ", ex);
                throw;
            }

        }
    }
}
