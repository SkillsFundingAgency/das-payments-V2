using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.DataLockService.Interfaces;
using SFA.DAS.Payments.DataLocks.Messages.Internal;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class ResetCacheHandler : IHandleMessages<ResetCacheCommand>
    {
        private readonly IPaymentLogger logger;
        private readonly IActorProxyFactory proxyFactory;

        public ResetCacheHandler(IPaymentLogger logger, IActorProxyFactory proxyFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        public async Task Handle(ResetCacheCommand message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogDebug($"Resetting cache for provider :{message.Ukprn}");
                var actorId = new ActorId(message.Ukprn);
                logger.LogVerbose($"Creating actor proxy for provider with UKPRN {message.Ukprn}");
                var actor = proxyFactory.CreateActorProxy<IDataLockService>(new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/DataLockServiceActorService"), actorId);
                logger.LogDebug($"Actor proxy created for UKPRN {message.Ukprn}, now resetting the cache.");
                await actor.Reset().ConfigureAwait(false);
                logger.LogInfo($"Finished resetting the cache for provider: {message.Ukprn}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed resetting cache for provider: {message.Ukprn}, Error: {ex.Message}", ex);
                throw;
            }

        }
    }
}