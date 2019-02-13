using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    public interface IEarningEventHandler
    {
        Task<ReadOnlyCollection<RequiredPaymentEvent>> HandleEarningEvent(EarningEvent earningEvent, IDataCache<PaymentHistoryEntity[]> repositoryCache, CancellationToken cancellationToken);
    }
}