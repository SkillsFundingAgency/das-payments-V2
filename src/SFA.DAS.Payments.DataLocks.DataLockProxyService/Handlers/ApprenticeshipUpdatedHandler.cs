using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.DataLockService.Interfaces;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class ApprenticeshipUpdatedHandler : IHandleMessages<ApprenticeshipUpdated>
    {
        private readonly IPaymentLogger logger;
        private readonly IActorProxyFactory actorProxyFactory;

        public ApprenticeshipUpdatedHandler(IPaymentLogger logger, IActorProxyFactory actorProxyFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task Handle(ApprenticeshipUpdated message, IMessageHandlerContext context)
        {
            if (message.Uln == 0)
            {
                throw new InvalidOperationException("Invalid 'ApprenticeshipUpdated' received. Uln was 0.");
            }

            logger.LogDebug($"Now handling the apprenticeship updated event.  Apprenticeship: {message.Id}, employer: {message.EmployerAccountId}, ukprn: {message.Ukprn}");
            var actorId = new ActorId(message.Ukprn);
            logger.LogVerbose($"Creating actor proxy.");
            var actor = actorProxyFactory.CreateActorProxy<IDataLockService>(new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/DataLockServiceActorService"), actorId);
            logger.LogDebug($"Actor proxy created for actor id {message.Uln}");
            await actor.HandleApprenticeshipUpdated(message, CancellationToken.None).ConfigureAwait(false);

            //var dataLockEvents = await actor.GetApprenticeshipUpdatedPayments( message, CancellationToken.None).ConfigureAwait(false);
            //logger.LogDebug($"Earning handled for learner with learner uln {message.Uln}");

            //if (dataLockEvents != null)
            //{
            //    var summary = string.Join(", ", dataLockEvents.GroupBy(e => e.GetType().Name).Select(g => $"{g.Key}: {g.Count()}"));
            //    logger.LogVerbose($"Publishing data lock event for learner with learner uln {message.Uln}: {summary}");
            //    await Task.WhenAll(dataLockEvents.Select(context.Publish)).ConfigureAwait(false);
            //    logger.LogDebug($"Data lock event published for learner with learner uln {message.Uln}");
            //}
                
            //var dataLockFunctionalSkillEvents = await actor.GetApprenticeshipUpdateFunctionalSkillPayments(message, CancellationToken.None).ConfigureAwait(false);
            //if (dataLockFunctionalSkillEvents != null)
            //{
            //    var summary = string.Join(", ", dataLockFunctionalSkillEvents.GroupBy(e => e.GetType().Name).Select(g => $"{g.Key}: {g.Count()}"));
            //    logger.LogVerbose($"Publishing data lock event for learner with learner uln {message.Uln}: {summary}");
            //    await Task.WhenAll(dataLockFunctionalSkillEvents.Select(context.Publish)).ConfigureAwait(false);
            //    logger.LogDebug($"Data lock event published for learner with learner uln {message.Uln}");
            //}
                
            logger.LogInfo($"Finished handling the apprenticeship updated event.  Apprenticeship: {message.Id}, employer: {message.EmployerAccountId}, provider: {message.Ukprn}");
        }
    }
}