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
        private readonly IPaymentLogger logger;

        public ApprenticeshipContractType1EarningEventHandler(IActorProxyFactory proxyFactory, IPaymentLogger logger)
        {
            this.proxyFactory = proxyFactory;
            this.logger = logger;
        }

        public async Task Handle(ApprenticeshipContractType1EarningEvent message, IMessageHandlerContext context)
        {
            try
            {
                if (message.Learner== null || message.Learner?.Uln == 0)
                {
                    throw new InvalidOperationException("Invalid 'ApprenticeshipContractType1EarningEvent' received. Learner was null or Uln was 0.");
                }
                var uln = message.Learner.Uln;
                var learnerRef = message.Learner.ReferenceNumber;
                logger.LogDebug($"Processing DataLockProxyProxyService event for learner with learn ref {learnerRef}");
                var actorId = new ActorId(uln);

                logger.LogVerbose($"Creating actor proxy for learner with leearner ref {learnerRef}");
                var actor = proxyFactory.CreateActorProxy<IDataLockService>(new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/DataLockServiceActorService"), actorId);
                logger.LogDebug($"Actor proxy created for learner with ULN {uln}");

                logger.LogVerbose($"Calling actor proxy to handle earning for learner with leearner ref {learnerRef}");
                var dataLockEvents = await actor.HandleEarning(message, CancellationToken.None).ConfigureAwait(false);
                logger.LogDebug($"Earning handled for learner with leearner ref {learnerRef}");

                if (dataLockEvents != null)
                {
                    var summary = string.Join(", ", dataLockEvents.GroupBy(e => e.GetType().Name).Select(g => $"{g.Key}: {g.Count()}"));
                    logger.LogVerbose($"Publishing data lock event for learner with leearner ref {learnerRef}: {summary}");
                    await Task.WhenAll(dataLockEvents.Select(context.Publish)).ConfigureAwait(false);
                    logger.LogDebug($"Data lock event published for learner with leearner ref {learnerRef}");
                }

                logger.LogInfo($"Successfully processed DataLockProxyProxyService event for Actor Id {actorId}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling ApprenticeshipContractType1EarningEvent message. Error: {ex.Message}.", ex);
                throw;
            }


        }
    }
}
