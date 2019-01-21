using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SFA.DAS.Payments.DataLocks.Messages;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

[assembly:FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces
{
    public interface IRequiredPaymentsService : IActor
    {
        Task<ReadOnlyCollection<RequiredPaymentEvent>> HandleApprenticeshipContractTypeEarningsEvent(ApprenticeshipContractTypeEarningsEvent earningEvent, CancellationToken cancellationToken);
        Task<ReadOnlyCollection<RequiredPaymentEvent>> HandleFunctionalSkillEarningsEvent(FunctionalSkillEarningsEvent earningEvent, CancellationToken cancellationToken);
        Task<ReadOnlyCollection<RequiredPaymentEvent>> HandlePayableEarningEvent(PayableEarningEvent earningEvent, CancellationToken cancellationToken);

        Task Initialise();

        Task Reset();
    }
}
