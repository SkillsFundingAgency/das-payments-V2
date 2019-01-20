using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IMonthEndEventHandlerService
    {
        Task<List<long>> GetMonthEndUkprns(CollectionPeriod collectionPeriod,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
