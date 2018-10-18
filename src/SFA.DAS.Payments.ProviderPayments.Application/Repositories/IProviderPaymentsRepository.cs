using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.Application.Repositories
{
    public interface IProviderPaymentsRepository
    {
        Task SavePayment(PaymentDataEntity paymentData, CancellationToken cancellationToken = default(CancellationToken));
    }
}
