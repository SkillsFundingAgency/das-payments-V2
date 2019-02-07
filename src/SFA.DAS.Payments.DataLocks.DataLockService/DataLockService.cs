using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.DataLockService.Interfaces;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class DataLockService : Actor, IDataLockService
    {
        private readonly IMapper mapper;
        private readonly IPaymentsDataContext dataContext;

        public DataLockService(
            ActorService actorService, 
            ActorId actorId, 
            IMapper mapper,
            IPaymentsDataContext dataContext) 
            : base(actorService, actorId)
        {
            this.mapper = mapper;
            this.dataContext = dataContext;
        }

        public async Task<DataLockEvent> HandleEarning(ApprenticeshipContractType1EarningEvent message, CancellationToken cancellationToken)
        {
            var commitment = dataContext.Commitment.FirstOrDefault(x => x.Uln == message.Learner.Uln);
            
            var returnMessage = mapper.Map<PayableEarningEvent>(message);
            returnMessage.EmployerAccountId = commitment.AccountId;
            return returnMessage;
        }
    }
}
