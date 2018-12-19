using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.RequiredPayments.Application;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    public class ReliableCollectionCache<T> : IRepositoryCache<T>
    {
        private readonly IActorStateManager stateManager;

        public ReliableCollectionCache(IActorStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        public async Task AddOrReplace(string key, T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await stateManager.AddOrUpdateStateAsync(key, entity, (oldKey, oldValue) => entity, cancellationToken);
        }

        public async Task<ConditionalValue<T>> TryGet(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await stateManager.TryGetStateAsync<T>(key, cancellationToken).ConfigureAwait(false);
            return new ConditionalValue<T>(result.HasValue, result.Value);
        }
    }
}