using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.DataLockService.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class ApprenticeshipContractType1FunctionalSkillEarningEventHandler: IHandleMessages<Act1FunctionalSkillEarningsEvent>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;

        public ApprenticeshipContractType1FunctionalSkillEarningEventHandler(IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger)
        {
            this.proxyFactory = proxyFactory;
            this.paymentLogger = paymentLogger;
        }

        public async Task Handle(Act1FunctionalSkillEarningsEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing DataLockProxyProxyService event for UKPRN {message.Ukprn}");

            var ukprn = message.Ukprn;
            var actorId = new ActorId(ukprn);

            paymentLogger.LogVerbose($"Creating actor proxy for provider with UKPRN {ukprn}");
            var actor = proxyFactory.CreateActorProxy<IDataLockService>(new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/DataLockServiceActorService"), actorId);
            paymentLogger.LogDebug($"Actor proxy created for UKPRN {ukprn}");

            paymentLogger.LogVerbose($"Calling actor proxy to handle functional skill earning for UKPRN {message.Ukprn}");
            var dataLockEvents = await actor.HandleFunctionalSkillEarning(message, CancellationToken.None).ConfigureAwait(false);
            paymentLogger.LogDebug($"Functional Skill Earning handled for UKPRN {message.Ukprn}");

            if (dataLockEvents != null)
            {
                var summary = string.Join(", ", dataLockEvents.GroupBy(e => e.GetType().Name).Select(g => $"{g.Key}: {g.Count()}"));
                paymentLogger.LogVerbose($"Publishing functional skill data lock event for UKPRN {message.Ukprn}: {summary}");
                await Task.WhenAll(dataLockEvents.Select(context.Publish)).ConfigureAwait(false);
                paymentLogger.LogDebug($"Functional Skill Data lock event published for UKPRN {message.Ukprn}");
            }

            paymentLogger.LogInfo($"Successfully processed DataLockProxyProxyService event for Actor Id {actorId}");
        }
    }
}
