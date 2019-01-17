using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Datalocks.DatalocksService.Interfaces;
using SFA.DAS.Payments.Datalocks.Messages;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Datalocks.DatalocksService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class DatalocksService : Actor, IDatalocksService
    {
      
        public DatalocksService(ActorService actorService, ActorId actorId): base(actorService, actorId)
        {
        }

        public Task<IDatalockPayment> HandlePayment(ApprenticeshipContractType1EarningEvent message, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
