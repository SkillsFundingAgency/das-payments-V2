using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.DataLockService.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Internal;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class ResetCacheHandler : IHandleMessages<ResetCacheCommand>
    {
        private readonly IPaymentLogger logger;
        private readonly IActorProxyFactory proxyFactory;
        private readonly IApprenticeshipRepository repository;

        public ResetCacheHandler(IPaymentLogger logger, IActorProxyFactory proxyFactory, IApprenticeshipRepository repository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Handle(ResetCacheCommand message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogDebug($"Resetting cache for provider :{message.Ukprn}");
                var ulns = await repository.ApprenticeshipUlnsByProvider(message.Ukprn);
                var resetTasks = new List<Task>();
                foreach (var uln in ulns)
                {
                    var actorId = new ActorId(uln);
                    logger.LogVerbose($"Creating actor proxy for actor id: {uln}");
                    var actor = proxyFactory.CreateActorProxy<IDataLockService>(new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/DataLockServiceActorService"), actorId);
                    logger.LogVerbose($"Actor proxy created, now resetting the cache.");
                    resetTasks.Add(actor.Reset());
                }

                await Task.WhenAll(resetTasks).ConfigureAwait(false);
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