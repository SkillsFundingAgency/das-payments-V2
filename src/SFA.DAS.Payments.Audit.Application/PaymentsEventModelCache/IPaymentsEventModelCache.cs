using System.Threading.Tasks;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache
{
    public interface IPaymentsEventModelCache<in T>  where T : PaymentsEventModel
    {
        Task AddPayment(T paymentsEventModel);
    }
}