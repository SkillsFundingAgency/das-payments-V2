using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.RequiredPayments.RemovedLearnerService.Interfaces
{
    public interface IRemovedLearnerService : IActor
    {
        Task<IList<IdentifiedRemovedLearningAim>> HandleIlrSubmittedEvent(short academicYear, byte collectionPeriod, DateTime ilrSubmissionDateTime, CancellationToken cancellationToken);
    }
}
