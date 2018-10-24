using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SFA.DAS.Payments.ProviderPayments.Model;
using System.Threading;
using System.Threading.Tasks;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Interfaces
{
    public interface IProviderPaymentsService : IActor
    {
        Task ProcessPayment(ProviderPeriodicPayment message, CancellationToken cancellationToken);

        Task HandleMonthEnd(short collectionYear, byte collectionPeriod, CancellationToken cancellationToken);
    }
}
