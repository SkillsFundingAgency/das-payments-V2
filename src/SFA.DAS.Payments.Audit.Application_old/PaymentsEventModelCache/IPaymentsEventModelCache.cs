using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache
{
    public interface IPaymentsEventModelCache
    {
        void AddPayment<T>(T paymentsEventModel) where T : PaymentsEventModel;
    }
}