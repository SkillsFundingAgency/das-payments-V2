using SFA.DAS.Payments.Model.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IProviderPaymentsService
    {
        Task ProcessPayment(PaymentModel payment, CancellationToken cancellationToken);
    }
}