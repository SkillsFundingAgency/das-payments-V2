using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IMonthEndEventHandlerService
    {
        Task<List<long>> GetMonthEndUkprns(int collectionYear, int collectionMonth, CancellationToken cancellationToken = default(CancellationToken));
    }
}
