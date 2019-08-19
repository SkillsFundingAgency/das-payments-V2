using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IHandleIlrSubmissionService
    {
        Task Handle(ReceivedProviderEarningsEvent message, CancellationToken cancellationToken);

        Task HandleSubmissionSucceeded(short academicYear, byte collectionPeriod, long ukprn, DateTime submissionTime,
            long jobId, CancellationToken cancellationToken);

        Task HandleSubmissionFailed(short academicYear, byte collectionPeriod, long ukprn, DateTime submissionTime,
            long jobId, CancellationToken cancellationToken);
    }
}