using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Messages;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    public interface IEarningEventProcessor<TEarningEvent>
        where TEarningEvent : IEarningEvent
    {
        Task<ReadOnlyCollection<RequiredPaymentEvent>> HandleEarningEvent(
            TEarningEvent earningEvent,
            IRepositoryCache<PaymentHistoryEntity[]> paymentHistoryCache,
            CancellationToken cancellationToken
        );
    }


    public interface IApprenticeshipContractTypeEarningsEventProcessor : IEarningEventProcessor<ApprenticeshipContractType2EarningEvent>
    {
    }

    public interface IFunctionalSkillEarningsEventProcessor : IEarningEventProcessor<FunctionalSkillEarningsEvent>
    {
    }

    public interface IPayableEarningEventProcessor : IEarningEventProcessor<PayableEarningEvent>
    {
    }
}