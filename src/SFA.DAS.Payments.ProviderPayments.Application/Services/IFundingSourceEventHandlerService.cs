using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IFundingSourceEventHandlerService
    {
        Task<List<long>> GetMonthEndUkprns(short collectionYear, byte collectionPeriod, CancellationToken cancellationToken = default(CancellationToken));
    }
}
