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
    public interface IApprenticeshipContractTypeEarningsEventProcessor
    {
        Task<ReadOnlyCollection<RequiredPaymentEvent>> ProcessApprenticeshipContractTypeEarningsEventEvent(ApprenticeshipContractTypeEarningsEvent earningEvent, IRepositoryCache<PaymentHistoryEntity[]> repositoryCache, CancellationToken cancellationToken);
    }

    public interface IFunctionalSkillEarningsEventProcessor
    {
        Task<ReadOnlyCollection<RequiredPaymentEvent>> ProcessFunctionalSkillEarningsEvent(FunctionalSkillEarningsEvent earningEvent, IRepositoryCache<PaymentHistoryEntity[]> repositoryCache, CancellationToken cancellationToken);
    }

    public interface IPayableEarningEventProcessor
    {
        Task<ReadOnlyCollection<RequiredPaymentEvent>> ProcessPayableEarningEvent(PayableEarningEvent earningEvent, IRepositoryCache<PaymentHistoryEntity[]> repositoryCache, CancellationToken cancellationToken);
    }
}