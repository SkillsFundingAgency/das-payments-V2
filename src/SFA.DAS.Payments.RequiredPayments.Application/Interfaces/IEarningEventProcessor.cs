using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    public interface IEarningEventProcessor<TEarningEvent>
        where TEarningEvent : IEarningEvent
    {
        Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleEarningEvent(
            TEarningEvent earningEvent,
            IDataCache<PaymentHistoryEntity[]> paymentHistoryCache,
            CancellationToken cancellationToken
        );
    }


    public interface IApprenticeshipContractType2EarningsEventProcessor : IEarningEventProcessor<ApprenticeshipContractTypeEarningsEvent>
    {
    }

    
    public interface IApprenticeshipAct1RedundancyEarningsEventProcessor : IEarningEventProcessor<ApprenticeshipContractType1RedundancyEarningEvent>
    {
    }


    public interface IFunctionalSkillEarningsEventProcessor : IEarningEventProcessor<IFunctionalSkillEarningEvent>
    {
    }

    public interface IPayableEarningEventProcessor : IEarningEventProcessor<PayableEarningEvent>
    {
    }
}