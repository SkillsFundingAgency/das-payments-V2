using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Application.Repositories
{
    public interface IDataCache<T>
    {
        Task<bool> Contains(string key, CancellationToken cancellationToken = default(CancellationToken));

        Task Add(string key, T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task AddOrReplace(string key, T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task<ConditionalValue<T>> TryGet(string key, CancellationToken cancellationToken = default(CancellationToken));

        Task Clear(string key = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
