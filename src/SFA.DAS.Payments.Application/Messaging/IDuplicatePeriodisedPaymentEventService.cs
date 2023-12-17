using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Messages.Common.Events;

namespace SFA.DAS.Payments.Application.Messaging
{
    public interface IDuplicatePeriodisedPaymentEventService
    {
        Task<bool> IsDuplicate(IPeriodisedPaymentEvent periodisedPaymentEvent, CancellationToken cancellationToken);
    }
}