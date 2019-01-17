using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Datalocks.DatalocksService.Interfaces;

namespace SFA.DAS.Payments.Datalocks.DatalocksService
{

    [StatePersistence(StatePersistence.Persisted)]
    public class DatalocksService : Actor, IDatalocksService
    {
      
        public DatalocksService(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            return this.StateManager.TryAddStateAsync("count", 0);
        }

        
        Task<int> IDatalocksService.GetCountAsync(CancellationToken cancellationToken)
        {
            return this.StateManager.GetStateAsync<int>("count", cancellationToken);
        }

    
        Task IDatalocksService.SetCountAsync(int count, CancellationToken cancellationToken)
        {
          
            return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value, cancellationToken);
        }
    }
}
