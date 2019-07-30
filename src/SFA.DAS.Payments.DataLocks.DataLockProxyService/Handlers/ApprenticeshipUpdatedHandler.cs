using System;
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
            try
            {
                if (message.Uln == 0)
                {
                    throw new InvalidOperationException("Invalid 'ApprenticeshipUpdated' received. Uln was 0.");
                }
                
                logger.LogDebug($"Now handling the apprenticeship updated event.  Apprenticeship: {message.Id}, employer: {message.EmployerAccountId}, ukprn: {message.Ukprn}, learner with ULN {message.Uln.ToString().Substring(0, 4)}");
                var actorId = new ActorId(message.Ukprn);
                logger.LogVerbose($"Creating actor proxy for provider with ULN {message.Uln.ToString().Substring(0,4)}");
                var actor = actorProxyFactory.CreateActorProxy<IDataLockService>(new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/DataLockServiceActorService"), actorId);
                logger.LogDebug($"Actor proxy created for ULN {message.Uln.ToString().Substring(0, 4)}");
                await actor.HandleApprenticeshipUpdated(message, CancellationToken.None).ConfigureAwait(false);
                logger.LogInfo($"Finished handling the apprenticeship updated event.  Apprenticeship: {message.Id}, employer: {message.EmployerAccountId}, provider: {message.Ukprn}, ULN {message.Uln.ToString().Substring(0, 4)}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to handle the apprenticeship updated event. Apprenticeship: {message.Id}, employer: {message.EmployerAccountId}, provider: {message.Ukprn}, ULN: {message.Uln.ToString().Substring(0, 4)}. Error: {ex}", ex);
                throw;
            }
        }
    }
}