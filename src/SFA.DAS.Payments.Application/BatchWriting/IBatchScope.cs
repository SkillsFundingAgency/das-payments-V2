using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Application.BatchWriting
{
    public interface IBatchScope: IDisposable
    {
        IBatchProcessor<T> GetBatchProcessor<T>();
        void Abort();
        Task Commit();
    }
}