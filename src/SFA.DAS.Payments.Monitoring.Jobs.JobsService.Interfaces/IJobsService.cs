using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.Monitoring.Jobs.JobsService.Interfaces
{
    public interface IJobsService : IActor
    {
        Task RecordEarningsJob(RecordEarningsJob message, CancellationToken cancellationToken);
        Task<JobStatus> RecordJobMessageProcessingStatus(RecordJobMessageProcessingStatus message, CancellationToken cancellationToken);
        Task RecordJobMessageProcessingStartedStatus(RecordStartedProcessingJobMessages message,
            CancellationToken cancellationToken);
    }
}
