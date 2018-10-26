using System.Collections.Generic;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SFA.DAS.Payments.ProviderPayments.Model;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Interfaces
{
    public interface IProviderPaymentsService : IActor
    {
        Task ProcessPayment(PaymentModel message, CancellationToken cancellationToken);

        Task<List<PaymentModel>> HandleMonthEnd(short collectionYear, byte collectionPeriod, CancellationToken cancellationToken);
    }
}
