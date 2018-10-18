using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IFundingSourceEventHandlerService
    {
        Task ProcessEvent(FundingSourcePaymentEvent message,CancellationToken cancellationToken = default(CancellationToken));

    }
}
