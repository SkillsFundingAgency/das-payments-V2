using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.Monitoring.Jobs.JobsService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

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
    }
}
