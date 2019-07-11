using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.DataLocks.DataLockService.Interfaces
{
    public interface IDataLockService : IActor
    {
        Task<List<DataLockEvent>> HandleEarning(ApprenticeshipContractType1EarningEvent message,  CancellationToken cancellationToken);
        Task HandleApprenticeshipUpdated(ApprenticeshipUpdated message, CancellationToken none);
        Task Reset();
    }
}
