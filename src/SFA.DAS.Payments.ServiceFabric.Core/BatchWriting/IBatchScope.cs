using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ServiceFabric.Core.BatchWriting
{
    public interface IBatchScope: IDisposable
    {
        IBatchProcessor<T> GetBatchProcessor<T>();
        void Abort();
        Task Commit();
    }
}