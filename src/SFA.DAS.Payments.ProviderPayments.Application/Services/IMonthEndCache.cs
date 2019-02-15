using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IMonthEndCache
    {
        Task AddOrReplace(long ukprn, short academicYear, byte collectionPeriod, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> Exists(long ukprn, short academicYear, byte collectionPeriod, CancellationToken cancellationToken = default(CancellationToken));
    }
}