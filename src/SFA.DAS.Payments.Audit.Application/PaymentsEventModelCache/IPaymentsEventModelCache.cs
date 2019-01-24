using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache
{
    public interface IPaymentsEventModelCache<T>  where T : IPaymentsEventModel
    {
        Task AddPayment(T paymentsEventModel);
        Task<List<T>> GetPayments(int batchSize);
    }
}