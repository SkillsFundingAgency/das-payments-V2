using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache
{
    public interface IPaymentsEventModelCache<T> : IBatchedDataCache<T>  where T : IPaymentsEventModel
    {
    }
}