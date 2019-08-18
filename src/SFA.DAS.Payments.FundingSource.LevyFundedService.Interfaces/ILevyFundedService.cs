using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]

namespace SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface ILevyFundedService : IActor
    {
        Task HandleRequiredPayment(CalculatedRequiredLevyAmount message);
        Task<ReadOnlyCollection<FundingSourcePaymentEvent>> UnableToFundTransfer(ProcessUnableToFundTransferFundingSourcePayment message);
        Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(ProcessLevyPaymentsOnMonthEndCommand command);
        Task HandleEmployerProviderPriorityChange(EmployerChangedProviderPriority message);
        Task RemovePreviousSubmissions(ProcessPreviousSubmissionDeletionCommand command);
        Task RemoveCurrentSubmission(ProcessCurrentSubmissionDeletionCommand command);
    }
}
