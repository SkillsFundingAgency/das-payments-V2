using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Application.Repositories
{
    public interface IActorDataCache<T> : IDataCache<T>
    {
        Task Initialise(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> IsInitialised(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> IsEmpty(CancellationToken cancellationToken = default(CancellationToken));
        Task Reset(CancellationToken cancellationToken = default(CancellationToken));
    }
}