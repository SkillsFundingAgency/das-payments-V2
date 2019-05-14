using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.RequiredPayments.RemovedLearnerService.Interfaces
{
    public interface IRemovedLearnerService : IActor
    {
        Task HandleIlrSubmittedEvent(short academicYear, byte collectionPeriod, DateTime ilrSubmissionDateTime);
    }
}
