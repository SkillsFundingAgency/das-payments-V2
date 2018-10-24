using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IMonthEndEventHandlerService
    {
        Task<List<ProviderPaymentEvent>> GetMonthEndPaymentsAsync(short collectionYear, byte collectionPeriod, long ukprn, CancellationToken cancellationToken = default(CancellationToken));
    }
}
