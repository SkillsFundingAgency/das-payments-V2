using SFA.DAS.Payments.Application.BatchWriting;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.ServiceFabric.Core.BatchWriting;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache
{
    public interface IPaymentsEventModelCache<T> : IBatchedDataCache<T>  where T : IPaymentsEventModel
    {
    }
}