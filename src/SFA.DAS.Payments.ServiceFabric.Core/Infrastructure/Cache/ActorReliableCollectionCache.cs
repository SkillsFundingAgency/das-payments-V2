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
        private const string InitialisedKey = "ACTOR_CACHE_INITIALISED";

        public ActorReliableCollectionCache(IActorStateManagerProvider actorStateManagerProvider) : base(actorStateManagerProvider)
        {
        }

        public ActorReliableCollectionCache(IActorStateManager stateManager) : base(stateManager)
        {
        }

        public async Task SetInitialiseFlag(CancellationToken cancellationToken = default(CancellationToken))
        {
             await StateManager.AddStateAsync(InitialisedKey, true, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> IsInitialiseFlagIsSet(CancellationToken cancellationToken = default(CancellationToken))
        {
           var isInitialised = await this.Contains(InitialisedKey, cancellationToken).ConfigureAwait(false);
            return isInitialised;
        }

        public async Task<bool> IsEmpty(CancellationToken cancellationToken = default(CancellationToken))
        {
            var keys = await StateManager.GetStateNamesAsync(cancellationToken).ConfigureAwait(false);
            return (keys == null || keys.All(o => o.Equals(InitialisedKey)));
        }

        public async Task ResetInitialiseFlag(CancellationToken cancellationToken = default(CancellationToken))
        {
            await StateManager.TryRemoveStateAsync(InitialisedKey, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
