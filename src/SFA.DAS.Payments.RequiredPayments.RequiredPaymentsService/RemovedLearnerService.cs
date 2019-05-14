using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    [StatePersistence(StatePersistence.None)]
    public class RemovedLearnerService  : Actor, IRemovedLearnerService
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
