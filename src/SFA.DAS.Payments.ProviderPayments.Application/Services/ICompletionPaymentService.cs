using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface ICompletionPaymentService
    {
        Task<IList<RecordedAct1CompletionPayment>> GetAct1CompletionPaymentEvents(ProcessMonthEndAct1CompletionPaymentCommand message);
    }
}