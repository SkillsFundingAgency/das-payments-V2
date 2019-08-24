using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.Monitoring.JobsService.Interfaces
{
    public interface IJobsService : IActor
    {
        Task RecordEarningsJob(RecordEarningsJob message);
    }
}
