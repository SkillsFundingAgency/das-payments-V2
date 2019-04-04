using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache
{
    public class ActorReliableCollectionCache<T>: ReliableCollectionCache<T>, IActorDataCache<T>
    {
        private const string initialisedKey = "ACTOR_CACHE_INITIALISED";

        public ActorReliableCollectionCache(IActorStateManagerProvider actorStateManagerProvider) : base(actorStateManagerProvider)
        {
        }

        public ActorReliableCollectionCache(IActorStateManager stateManager) : base(stateManager)
        {
        }

        public async Task Initialise(CancellationToken cancellationToken = default(CancellationToken))
        {
             await StateManager.AddStateAsync(initialisedKey, true, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> IsInitialised(CancellationToken cancellationToken = default(CancellationToken))
        {
           var isInitialised = await this.Contains(initialisedKey, cancellationToken);
            return isInitialised;
        }

        public async Task<bool> IsEmpty(CancellationToken cancellationToken = default(CancellationToken))
        {
            var keys = await StateManager.GetStateNamesAsync(cancellationToken);
            return (keys == null || keys.All(o => o.Equals(initialisedKey)));
        }

        public async Task Reset(CancellationToken cancellationToken = default(CancellationToken))
        {
            await StateManager.TryRemoveStateAsync(initialisedKey, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
