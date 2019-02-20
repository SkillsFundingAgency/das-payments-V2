using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache
{
    public class ReliableCollectionCache<T> : IDataCache<T>
    {
        private readonly IActorStateManagerProvider actorStateManagerProvider;
        private IActorStateManager stateManager;
        
        public IActorStateManager StateManager {
            get
            {
                if (stateManager == null)
                    stateManager = actorStateManagerProvider.Current;
                return stateManager;
            }
        }

        public ReliableCollectionCache(IActorStateManagerProvider actorStateManagerProvider)
        {
            this.actorStateManagerProvider = actorStateManagerProvider;
        }


        // TODO: this is to go when RequiredPayments uses IActorStateManagerProvider
        public ReliableCollectionCache(IActorStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        public async Task<bool> Contains(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await StateManager.ContainsStateAsync(key, cancellationToken).ConfigureAwait(false);
        }

        public async Task Add(string key, T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await StateManager.AddStateAsync(key, entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddOrReplace(string key, T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await StateManager.AddOrUpdateStateAsync(key, entity, (oldKey, oldValue) => entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ConditionalValue<T>> TryGet(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await StateManager.TryGetStateAsync<T>(key, cancellationToken).ConfigureAwait(false);
            return new ConditionalValue<T>(result.HasValue, result.Value);
        }

        public async Task Clear(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            await StateManager.TryRemoveStateAsync(key, cancellationToken).ConfigureAwait(false);
        }
    }
}