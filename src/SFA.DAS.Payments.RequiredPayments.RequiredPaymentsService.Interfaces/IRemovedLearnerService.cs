using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces
{
    public interface IRemovedLearnerService : IActor
    {
        Task HandleIlrSubmittedEvent(short academicYear, byte collectionPeriod, DateTime ilrSubmissionDateTime);
    }
}
