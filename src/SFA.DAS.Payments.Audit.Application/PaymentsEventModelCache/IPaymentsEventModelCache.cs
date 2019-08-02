using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache
{
    public interface IPaymentsEventModelCache<T> : IBatchedDataCache<T>  where T : IPaymentsEventModel
    {
    }
}