using System.Threading.Tasks;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IProcessAfterMonthEndPaymentService
    {
        Task<ProviderPaymentEvent> GetPaymentEvent(FundingSourcePaymentEvent message);
    }
}