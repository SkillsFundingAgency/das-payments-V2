using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IMonthEndCache
    {
        Task<bool> Exists(long ukprn, short academicYear, byte collectionPeriod, CancellationToken cancellationToken = default(CancellationToken));
        Task<MonthEndDetails> GetMonthEndDetails(long ukprn, short academicYear,byte collectionPeriod, CancellationToken cancellationToken = default(CancellationToken));
    }
}