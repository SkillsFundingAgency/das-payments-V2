using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Messages.Common.Events;

namespace SFA.DAS.Payments.Application.Messaging
{
    public interface IDuplicateEarningEventService
    {
        Task<bool> IsDuplicate(IPaymentsEvent earningEvent, CancellationToken cancellationToken);
    }
}