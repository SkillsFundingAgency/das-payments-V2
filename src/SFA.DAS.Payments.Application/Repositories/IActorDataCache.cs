using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Application.Repositories
{
    public interface IActorDataCache<T> : IDataCache<T>
    {
        Task SetInitialiseFlag(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> IsInitialiseFlagIsSet(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> IsEmpty(CancellationToken cancellationToken = default(CancellationToken));
        Task ResetInitialiseFlag(CancellationToken cancellationToken = default(CancellationToken));
    }
}