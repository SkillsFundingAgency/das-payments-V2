using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class CollectionStartedEventHandler : IHandleMessages<CollectionStartedEvent>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;

        public CollectionStartedEventHandler(IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger)
        {
            this.proxyFactory = proxyFactory;
            this.paymentLogger = paymentLogger;
        }

        public async Task Handle(CollectionStartedEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing CollectionStartedEvent event. Message Id : {context.MessageId}");

            //foreach (var key in message.ApprenticeshipKeys)
            //{
            //    var actorId = new ActorId(key);
            //    var actor = proxyFactory.CreateActorProxy<IRequiredPaymentsService>(new Uri("fabric:/SFA.DAS.Payments.RequiredPayments.ServiceFabric/RequiredPaymentsServiceActorService"), actorId);
                
            //    //await actor.Reset().ConfigureAwait(false);

            //    paymentLogger.LogVerbose($"Successfully processed CollectionStartedEvent event for Actor Id {actorId}");
            //}

            await context.Reply(0);
        }
    }
}
