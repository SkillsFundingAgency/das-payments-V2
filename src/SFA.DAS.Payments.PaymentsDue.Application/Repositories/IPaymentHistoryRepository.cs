using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;

namespace SFA.DAS.Payments.PaymentsDue.Application.Repositories
{
    public interface IPaymentHistoryRepository
    {
        Task<IEnumerable<Payment>> GetPaymentHistory(string apprenticeshipKey, CancellationToken cancellationToken = default(CancellationToken));
    }
}
