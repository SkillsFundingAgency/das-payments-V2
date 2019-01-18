using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    public interface IEarningEventHandler
    {
        Task<ReadOnlyCollection<RequiredPaymentEvent>> HandleEarningEvent(EarningEvent earningEvent, IRepositoryCache<PaymentHistoryEntity[]> repositoryCache, CancellationToken cancellationToken);
    }
}