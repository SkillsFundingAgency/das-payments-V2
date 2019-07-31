using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.DataLockService.Interfaces;
using SFA.DAS.Payments.DataLocks.Messages.Internal;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class ResetActorsHandler: IHandleMessages<ResetActorsCommand>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;

        public ResetActorsHandler(IPaymentLogger logger, IActorProxyFactory proxyFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        public async Task Handle(ResetActorsCommand message, IMessageHandlerContext context)
        {
            logger.LogDebug("Resetting datalock actors.");
            var resetTasks = new List<Task>();
            foreach (var uln in message.Ulns)
            {
                var actorId = new ActorId(uln);
                logger.LogVerbose($"Creating actor proxy, actor id: {uln}.");
                var actor = proxyFactory.CreateActorProxy<IDataLockService>(new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/DataLockServiceActorService"), actorId);
                logger.LogVerbose($"Actor proxy created. Actor id: {uln}, now resetting the cache.");
                resetTasks.Add(actor.Reset());
            }

            await Task.WhenAll(resetTasks).ConfigureAwait(false);
            logger.LogInfo("Finished resetting the datalock actors");
        }
    }
}