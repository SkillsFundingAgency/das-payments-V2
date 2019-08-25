using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Monitoring.Jobs.JobsService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsService
{

    [StatePersistence(StatePersistence.Volatile)]
    public class JobsService : Actor, IJobsService
    {

        public JobsService(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        public Task RecordEarningsJob(RecordEarningsJob message)
        {
            throw new NotImplementedException();
        }

        public Task<JobStatus> RecordJobMessageProcessingStatus(RecordJobMessageProcessingStatus message)
        {
            throw new NotImplementedException();
        }
    }
}
