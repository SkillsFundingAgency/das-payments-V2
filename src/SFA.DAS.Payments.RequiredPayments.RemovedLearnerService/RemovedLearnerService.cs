using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.RequiredPayments.RemovedLearnerService.Interfaces;

namespace SFA.DAS.Payments.RequiredPayments.RemovedLearnerService
{
    [StatePersistence(StatePersistence.None)]
    internal class RemovedLearnerService : Actor, IRemovedLearnerService
    {
        private long ukprn;

        public RemovedLearnerService(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
            ukprn = actorId.GetLongId();
        }

        public async Task HandleIlrSubmittedEvent(short academicYear, byte collectionPeriod, DateTime ilrSubmissionDateTime)
        {
            return;
        }
    }
}
