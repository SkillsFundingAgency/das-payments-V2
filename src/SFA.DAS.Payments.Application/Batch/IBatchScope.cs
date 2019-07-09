using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Application.Batch
{
    public interface IBatchScope: IDisposable
    {
        IBatchProcessor<T> GetBatchProcessor<T>();
        void Abort();
        Task Commit();
    }
}