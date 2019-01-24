using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class MonthEndEventHandlerService : IMonthEndEventHandlerService
    {
        private readonly IProviderPaymentsRepository providerPaymentsRepository;

        public MonthEndEventHandlerService(IProviderPaymentsRepository providerPaymentsRepository)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
        }

        public Task<List<long>> GetMonthEndUkprns(CollectionPeriod collectionPeriod,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return providerPaymentsRepository.GetMonthEndProviders(collectionPeriod, cancellationToken);
        }
    }
}
