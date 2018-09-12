using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.RequiredPayments.Application;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    public class ReliableCollectionCache<T> : IRepositoryCache<T>
    {
        private readonly IActorStateManager _stateManager;

        public ReliableCollectionCache(IActorStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public async Task<bool> Contains(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _stateManager.ContainsStateAsync(key, cancellationToken).ConfigureAwait(false);
        }

        public async Task Add(string key, T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _stateManager.AddStateAsync(key, entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ConditionalValue<T>> TryGet(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _stateManager.TryGetStateAsync<T>(key, cancellationToken).ConfigureAwait(false);
            return new ConditionalValue<T>(result.HasValue, result.Value);
        }

        public async Task Clear(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _stateManager.TryRemoveStateAsync(key, cancellationToken).ConfigureAwait(false);            
        }
    }
}