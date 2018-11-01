using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IIlrSubmissionService
    {
        Task HandleIlrSubMission(IlrSubmittedEvent message, CancellationToken cancellationToken);
    }
}