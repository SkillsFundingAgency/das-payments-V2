using System.Collections.Generic;
using SFA.DAS.Payments.ProviderPayments.Model;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IFundingSourceEventHandlerService
    {
        Task ProcessEvent(ProviderPeriodicPayment message, CancellationToken cancellationToken = default(CancellationToken));
       
    }
}
