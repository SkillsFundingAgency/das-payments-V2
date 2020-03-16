using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IPaymentCompletionService
    {
        Task<IList<RecordedAct1CompletionPaymentEvent>> GetAct1CompletionPaymentEvents(ProcessMonthEndAct1CompletionPaymentCommand message);
    }
}