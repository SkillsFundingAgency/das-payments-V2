using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    public interface IRemovedLearnerAimIdentificationService
    {
        Task<List<IdentifiedRemovedLearningAim>> IdentifyRemovedLearnerAims(short academicYear, byte collectionPeriod, long ukprn, CancellationToken cancellationToken);
    }
}
