using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.PeriodEnd.TestEndpoint.Application.Services
{
    public interface IBuildMonthEndPaymentEvent
    {
        Task<CollectionStartedEvent> CreateCollectionStartedEvent(long ukprn, short academicYear);
        ProcessProviderMonthEndCommand CreateProcessProviderMonthEndCommand(long ukprn, short academicYear, byte period);
    }
}
