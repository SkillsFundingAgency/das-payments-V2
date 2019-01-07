using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Interfaces
{
    public interface IProviderPaymentsService : IActor
    {
        Task HandlePayment(PaymentModel message, CancellationToken cancellationToken);

        Task<List<PaymentModel>> GetMonthEndPayments(int collectionYear, int collectionPeriod, CancellationToken cancellationToken);

        Task HandleIlrSubMission(IlrSubmittedEvent message, CancellationToken cancellationToken);
    }
}
