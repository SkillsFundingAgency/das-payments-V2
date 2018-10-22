using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using System.Threading;
using System.Threading.Tasks;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Interfaces
{
    public interface IProviderPaymentsService : IActor
    {
        Task HandleEvent(FundingSourcePaymentEvent message, CancellationToken cancellationToken);
    }
}
