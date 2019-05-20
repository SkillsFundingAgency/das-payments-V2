using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.DataLockService.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class ApprenticeshipContractType1EarningEventHandler : IHandleMessages<ApprenticeshipContractType1EarningEvent>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;

        public ApprenticeshipContractType1EarningEventHandler(IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger)
        {
            this.proxyFactory = proxyFactory;
            this.paymentLogger = paymentLogger;
        }

        public async Task Handle(ApprenticeshipContractType1EarningEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing DataLockProxyProxyService event. Message Id : {context.MessageId}");

            var ukprn = message.Ukprn;
            var actorId = new ActorId(ukprn);

            paymentLogger.LogVerbose($"Creating actor proxy for provider with UKPRN {ukprn}");
            var actor = proxyFactory.CreateActorProxy<IDataLockService>(new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/DataLockServiceActorService"), actorId);
            paymentLogger.LogDebug($"Actor proxy created for provider with UKPRN {ukprn}");

            paymentLogger.LogVerbose("Calling actor proxy to handle earning");
            var dataLockEvents = await actor.HandleEarning(message, CancellationToken.None).ConfigureAwait(false);
            paymentLogger.LogDebug("Earning handled");

            if (dataLockEvents != null)
            {
                paymentLogger.LogVerbose("Publishing data lock event");
                await Task.WhenAll(dataLockEvents.Select(context.Publish)).ConfigureAwait(false);
                paymentLogger.LogDebug("Data lock event published");
            }

            paymentLogger.LogInfo($"Successfully processed DataLockProxyProxyService event for Actor Id {actorId}");
        }
    }
}
