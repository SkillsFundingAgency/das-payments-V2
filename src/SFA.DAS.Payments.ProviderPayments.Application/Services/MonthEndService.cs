using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class MonthEndService : IMonthEndService
    {
        private readonly IProviderPaymentsRepository providerPaymentsRepository;

        public MonthEndService(IProviderPaymentsRepository providerPaymentsRepository)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
        }

        public async Task<List<PaymentModel>> GetMonthEndPayments(CollectionPeriod collectionPeriod,
            long ukprn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await providerPaymentsRepository.GetMonthEndPayments(collectionPeriod, ukprn, cancellationToken);
        }
    }
}