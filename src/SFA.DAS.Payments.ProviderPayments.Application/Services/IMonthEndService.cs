using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IMonthEndService
    {
        Task<List<PaymentModel>> GetMonthEndPayments(CollectionPeriod collectionPeriod, long ukprn,
            CancellationToken cancellationToken = default(CancellationToken));

        Task StartMonthEnd(long ukprn, short academicYear, byte collectionPeriod);
        Task<bool> MonthEndStarted(long ukprn, short academicYear, byte collectionPeriod);
    }
}