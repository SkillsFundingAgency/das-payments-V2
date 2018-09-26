using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Repositories
{
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly IRequiredPaymentsDataContext requiredPaymentsDataContext;

        public PaymentHistoryRepository(IRequiredPaymentsDataContext requiredPaymentsDataContext)
        {
            this.requiredPaymentsDataContext = requiredPaymentsDataContext;
        }

        public async Task<PaymentEntity[]> GetPaymentHistory(string apprenticeshipKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await requiredPaymentsDataContext.PaymentHistory
                .Where(p => p.ApprenticeshipKey == apprenticeshipKey)
                .ToArrayAsync(cancellationToken);
        }
    }
}