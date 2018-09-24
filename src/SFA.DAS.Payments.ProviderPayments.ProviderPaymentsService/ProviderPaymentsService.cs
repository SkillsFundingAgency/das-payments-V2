using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService
{

    [StatePersistence(StatePersistence.Volatile)]
    public class ProviderPaymentsService : Actor, IProviderPaymentsService
    {

        public ProviderPaymentsService(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }


        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }


        Task<int> IProviderPaymentsService.GetCountAsync(CancellationToken cancellationToken)
        {
            return this.StateManager.GetStateAsync<int>("count", cancellationToken);
        }


        Task IProviderPaymentsService.SetCountAsync(int count, CancellationToken cancellationToken)
        {
            // Requests are not guaranteed to be processed in order nor at most once.
            // The update function here verifies that the incoming count is greater than the current count to preserve order.
            return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value, cancellationToken);
        }
    }
}
