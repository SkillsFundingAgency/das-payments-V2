using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.DataLocks.DataLockService.Interfaces;
using SFA.DAS.Payments.DataLocks.Messages;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class DataLockService : Actor, IDataLockService
    {
      
        public DataLockService(ActorService actorService, ActorId actorId): base(actorService, actorId)
        {
        }

        public Task<IDataLockPayment> HandlePayment(ApprenticeshipContractType1EarningEvent message, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
