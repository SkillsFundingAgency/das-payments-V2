using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

[assembly:FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces
{
    public interface IRequiredPaymentsService : IActor
    {
        Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleApprenticeship2ContractTypeEarningsEvent(ApprenticeshipContractTypeEarningsEvent earningEvent, CancellationToken cancellationToken);
        Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandlePayableEarningEvent(PayableEarningEvent earningEvent, CancellationToken cancellationToken);
        Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> RefundRemovedLearningAim(IdentifiedRemovedLearningAim removedLearningAim, CancellationToken cancellationToken);
        Task Initialise(byte currentCollectionPeriod);

        Task Reset();
        Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleFunctionalSkillEarningsEvent(Act2FunctionalSkillEarningsEvent earningEvent, CancellationToken cancellationToken);
        Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandlePayableFunctionalSkillEarningsEvent(PayableFunctionalSkillEarningEvent earningEvent, CancellationToken cancellationToken);
    }
}
