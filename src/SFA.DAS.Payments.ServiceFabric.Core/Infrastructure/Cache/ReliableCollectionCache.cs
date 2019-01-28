using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache
{
    public class ReliableCollectionCache<T> : IDataCache<T>
    {
        private readonly IActorStateManager stateManager;

        public ReliableCollectionCache(IActorStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        public async Task<bool> Contains(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await stateManager.ContainsStateAsync(key, cancellationToken).ConfigureAwait(false);
        }

        public async Task Add(string key, T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await stateManager.AddStateAsync(key, entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddOrReplace(string key, T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await stateManager.AddOrUpdateStateAsync(key, entity, (oldKey, oldValue) => entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ConditionalValue<T>> TryGet(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await stateManager.TryGetStateAsync<T>(key, cancellationToken).ConfigureAwait(false);
            return new ConditionalValue<T>(result.HasValue, result.Value);
        }

        public async Task Clear(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            await stateManager.TryRemoveStateAsync(key, cancellationToken).ConfigureAwait(false);            
        }
    }
}