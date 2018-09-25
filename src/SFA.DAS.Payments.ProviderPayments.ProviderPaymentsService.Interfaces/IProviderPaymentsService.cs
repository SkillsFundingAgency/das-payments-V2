using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading;
using System.Threading.Tasks;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Interfaces
{
    public interface IProviderPaymentsService : IActor
    {
        Task<int> GetCountAsync(CancellationToken cancellationToken);

        Task SetCountAsync(int count, CancellationToken cancellationToken);
    }
}
