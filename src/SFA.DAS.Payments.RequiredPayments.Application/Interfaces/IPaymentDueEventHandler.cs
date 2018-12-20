using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    public interface IPaymentDueEventHandler
    {
        Task<RequiredPaymentEvent> HandlePaymentDue(PaymentDueEvent paymentDue, IRepositoryCache<PaymentHistoryEntity[]> repositoryCache, CancellationToken cancellationToken);
    }
}