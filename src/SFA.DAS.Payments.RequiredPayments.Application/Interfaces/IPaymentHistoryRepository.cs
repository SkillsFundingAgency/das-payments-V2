using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    public interface IPaymentHistoryRepository
    {
        Task<PaymentEntity[]> GetPaymentHistory(string apprenticeshipKey, CancellationToken cancellationToken = default(CancellationToken));
    }
}
