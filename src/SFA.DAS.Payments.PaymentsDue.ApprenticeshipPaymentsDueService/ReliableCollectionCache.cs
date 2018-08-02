using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.PaymentsDue.Application.Repositories;

namespace SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueService
{
    public class ReliableCollectionCache<T> : IRepositoryCache<T>
    {
        private readonly IActorStateManager _stateManager;

        public ReliableCollectionCache(IActorStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public bool IsInitialised { get; set; }

        public async Task Reset()
        {
            await _stateManager.ClearCacheAsync();
        }

        public async Task Add(string key, T entity)
        {
            await _stateManager.AddStateAsync(key, entity).ConfigureAwait(false);
        }

        public async Task<T> Get(string key)
        {
            return await _stateManager.GetStateAsync<T>(key).ConfigureAwait(false);
        }
    }
}