using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    public interface IRepositoryCache<T>
    {
        Task<bool> Contains(string key, CancellationToken cancellationToken = default(CancellationToken));

        Task Add(string key, T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task<ConditionalValue<T>> TryGet(string key, CancellationToken cancellationToken = default(CancellationToken));

        Task Clear(string key, CancellationToken cancellationToken = default(CancellationToken));
    }
}
