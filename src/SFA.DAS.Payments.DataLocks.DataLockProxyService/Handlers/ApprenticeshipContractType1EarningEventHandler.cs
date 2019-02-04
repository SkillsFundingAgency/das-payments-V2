using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.DataLockService.Interfaces;
using SFA.DAS.Payments.DataLocks.Messages;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class ApprenticeshipContractType1EarningEventHandler : IHandleMessages<ApprenticeshipContractType1EarningEvent>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;
        private readonly IExecutionContext executionContext;

        public ApprenticeshipContractType1EarningEventHandler(IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger, IExecutionContext executionContext)
        {
            this.proxyFactory = proxyFactory;
            this.paymentLogger = paymentLogger;
            this.executionContext = executionContext;
        }
        public async Task Handle(ApprenticeshipContractType1EarningEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing DataLockProxyProxyService event. Message Id : {context.MessageId}");

            try
            {
                var actorId = new ActorId(message.Ukprn);
                var actor = proxyFactory.CreateActorProxy<IDataLockService>(new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/DataLockServiceActorService"), actorId);
                DataLockEvent payableEarningEvents;
                try
                {
                    payableEarningEvents = await actor.HandlePayment(message, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    paymentLogger.LogError($"Error invoking levy funded actor. Error: {ex.Message}", ex);
                    throw;
                }

                try
                {
                    if (payableEarningEvents != null)
                        await context.Publish(payableEarningEvents);
                }
                catch (Exception ex)
                {
                    paymentLogger.LogError($"Error publishing the event: 'RequiredPaymentEvent'.  Error: {ex.Message}.", ex);
                    throw;
                }

                paymentLogger.LogInfo($"Successfully processed DataLockProxyProxyService event for Actor Id {actorId}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError("Error while handling DataLockProxyProxyService event", ex);
                throw;
            }
        }
    }
}
