using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Repositories
{
    public interface IProviderPaymentsRepository
    {
        Task SavePayment(PaymentDataEntity paymentData, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<PaymentDataEntity>> GetMonthEndPayments(short collectionYear, byte collectionPeriodMonth, long ukprn, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<long>> GetMonthEndUkprns(short collectionYear, byte collectionPeriodMonth,CancellationToken cancellationToken = default(CancellationToken));
    }
}
