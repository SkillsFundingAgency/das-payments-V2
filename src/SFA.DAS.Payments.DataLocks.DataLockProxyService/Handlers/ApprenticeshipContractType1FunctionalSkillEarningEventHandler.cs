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
    public class ApprenticeshipContractType1FunctionalSkillEarningEventHandler : IHandleMessages<Act1FunctionalSkillEarningsEvent>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;

        public ApprenticeshipContractType1FunctionalSkillEarningEventHandler(IActorProxyFactory proxyFactory, IPaymentLogger logger)
        {
            this.proxyFactory = proxyFactory;
            this.logger = logger;
        }

        public async Task Handle(Act1FunctionalSkillEarningsEvent message, IMessageHandlerContext context)
        {
            try
            {
                if (message.Learner == null || message.Learner?.Uln == 0)
                {
                    throw new InvalidOperationException("Invalid 'Act1FunctionalSkillEarningsEvent' received. Learner was null or Uln was 0.");
                }
                var uln = message.Learner.Uln;
                var learnerRef = message.Learner.ReferenceNumber;
                logger.LogDebug($"Processing DataLockProxyProxyService event for learner with learner ref {learnerRef}");
                var actorId = new ActorId(uln);

                logger.LogVerbose($"Creating actor proxy for provider with learner ref {learnerRef}");
                var actor = proxyFactory.CreateActorProxy<IDataLockService>(new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/DataLockServiceActorService"), actorId);
                logger.LogDebug($"Actor proxy created for learner with ULN {uln}");

                logger.LogVerbose($"Calling actor proxy to handle functional skill earning for learner with learner ref {learnerRef}");
                var dataLockEvents = await actor.HandleFunctionalSkillEarning(message, CancellationToken.None).ConfigureAwait(false);
                logger.LogDebug($"Functional skill earning handled for learner with learner ref {learnerRef}");

                if (dataLockEvents != null)
                {
                    var summary = string.Join(", ", dataLockEvents.GroupBy(e => e.GetType().Name).Select(g => $"{g.Key}: {g.Count()}"));
                    logger.LogVerbose($"Publishing functional skill data lock event for learner with learner ref {learnerRef}: {summary}");
                    await Task.WhenAll(dataLockEvents.Select(context.Publish)).ConfigureAwait(false);
                    logger.LogDebug($"Functional Skill Data lock event published for learner with learner ref {learnerRef}");
                }

                logger.LogInfo($"Successfully processed DataLockProxyProxyService event for Actor Id {actorId}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling Act1FunctionalSkillEarningsEvent message. Error: {ex.Message}.", ex);
                throw;
            }
        }
    }
}