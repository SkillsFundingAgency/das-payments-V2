using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ServiceFabric.Core.BatchWriting
{
    public interface IBatchProcessor<T>
    {
        Task<int> Process(int batchSize, CancellationToken cancellationToken);
    }
}