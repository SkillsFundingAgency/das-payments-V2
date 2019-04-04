using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    public interface IPaymentHistoryRepository
    {
        Task<List<PaymentHistoryEntity>> GetPaymentHistory(ApprenticeshipKey apprenticeshipKey, CancellationToken cancellationToken = default(CancellationToken));

        Task<decimal> GetEmployerCoInvestedPaymentHistoryTotal(ApprenticeshipKey apprenticeshipKey, CancellationToken cancellationToken = default(CancellationToken));
    }
}
