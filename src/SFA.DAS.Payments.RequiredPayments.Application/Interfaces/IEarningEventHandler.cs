using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    public interface IEarningEventHandler
    {
        Task<IReadOnlyCollection<RequiredPaymentEvent>> HandleEarningEvent(EarningEvent earningEvent, IRepositoryCache<PaymentHistoryEntity[]> repositoryCache, CancellationToken cancellationToken);
    }
}