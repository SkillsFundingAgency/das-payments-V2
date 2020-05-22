using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Domain.Interfaces
{
    public interface IDuplicateLearnerService
    {
        Task<bool> IsDuplicate(ProcessLearnerCommand processLearnerCommand, CancellationToken cancellationToken);
    }
}