using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands
{
    public class ProcessProviderMonthEndCommand: PaymentsCommand, IMonitoredMessage
    {
        public long Ukprn { get; set; }
        public CollectionPeriod CollectionPeriod { get; set; }
    }
}
