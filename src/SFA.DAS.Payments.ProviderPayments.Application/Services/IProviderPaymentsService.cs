using SFA.DAS.Payments.Model.Core.Entities;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IProviderPaymentsService
    {
        Task ProcessPayment(ProviderPaymentEventModel payment, CancellationToken cancellationToken);

        Task RemoveObsoletePayments(InvalidatedPayableEarningEvent message, CancellationToken cancellationToken);

    }
}