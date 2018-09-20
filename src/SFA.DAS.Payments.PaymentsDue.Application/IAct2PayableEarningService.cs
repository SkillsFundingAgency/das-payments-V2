using System.Threading.Tasks;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Application
{
    public interface IAct2PayableEarningService
    {
        Task<ApprenticeshipContractType2PaymentDueEvent[]> CreatePaymentsDue(ApprenticeshipContractType2EarningEvent message);
    }
}