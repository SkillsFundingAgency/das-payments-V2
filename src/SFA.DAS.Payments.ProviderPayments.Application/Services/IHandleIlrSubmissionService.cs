using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IHandleIlrSubmissionService
    {
        Task Handle(ReceivedProviderEarningsEvent message, CancellationToken cancellationToken);
        Task HandleSubmissionSucceeded(SubmissionSucceededEvent message, CancellationToken cancellationToken);
        Task HandleSubmissionFailed(SubmissionFailedEvent message, CancellationToken cancellationToken);
    }
}