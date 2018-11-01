using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IProviderPaymentsHandlerService
    {
        Task ProcessPayment(PaymentModel payment, CancellationToken cancellationToken);
        Task<List<PaymentModel>> GetMonthEndPayments(short collectionYear, byte collectionPeriod, long ukprn, CancellationToken cancellationToken = default(CancellationToken));
        Task HandleIlrSubMission(IlrSubmittedEvent message, CancellationToken cancellationToken);
    }
}