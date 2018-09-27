using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Application
{
    public class ApprenticeshipContractType2PayableEarningService : IApprenticeshipContractType2PayableEarningService
    {
        public Task<ApprenticeshipContractType2PaymentDueEvent[]> CreatePaymentsDue(ApprenticeshipContractType2EarningEvent message)
        {
            var 
            return Task.FromResult(new ApprenticeshipContractType2PaymentDueEvent[0]);
        }
    }
}
