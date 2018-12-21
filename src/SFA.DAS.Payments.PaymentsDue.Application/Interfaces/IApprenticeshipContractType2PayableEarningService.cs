using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Application
{
    public interface IApprenticeshipContractType2PayableEarningService
    {
        PaymentDueEvent[] CreatePaymentsDue(ApprenticeshipContractType2EarningEvent message);
    }
}