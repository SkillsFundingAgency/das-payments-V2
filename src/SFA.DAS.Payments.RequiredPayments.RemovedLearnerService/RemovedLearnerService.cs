using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.RequiredPayments.Application;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RemovedLearnerService.Interfaces;

namespace SFA.DAS.Payments.RequiredPayments.RemovedLearnerService
{
    [StatePersistence(StatePersistence.None)]
    public class RemovedLearnerService : Actor, IRemovedLearnerService
    {
        private readonly long ukprn;
        private readonly IRemovedLearnerAimIdentificationService removedLearnerAimIdentificationService;

        public RemovedLearnerService(ActorService actorService, ActorId actorId, IRemovedLearnerAimIdentificationService removedLearnerAimIdentificationService) : base(actorService, actorId)
        {
            this.removedLearnerAimIdentificationService = removedLearnerAimIdentificationService;

            if (!long.TryParse(actorId.GetStringId(), out ukprn))
            {
                throw new InvalidCastException($"Unable to cast Actor Id {Id.GetStringId()} to valid ukprn");
            }
        }

        public async Task<IList<IdentifiedRemovedLearningAim>> HandleReceivedProviderEarningsEvent(short academicYear, byte collectionPeriod, DateTime ilrSubmissionDateTime, CancellationToken cancellationToken)
        {
            return await removedLearnerAimIdentificationService.IdentifyRemovedLearnerAims(academicYear, collectionPeriod, ukprn, ilrSubmissionDateTime, cancellationToken).ConfigureAwait(false);
        }
    }
}

