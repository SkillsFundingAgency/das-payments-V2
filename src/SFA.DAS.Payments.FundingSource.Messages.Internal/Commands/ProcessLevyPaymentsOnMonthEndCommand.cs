using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.FundingSource.Messages.Internal.Commands
{
    public class ProcessLevyPaymentsOnMonthEndCommand : PaymentsCommand, IMonitoredMessage
    {
        public CollectionPeriod CollectionPeriod { get; set; }

        public long AccountId { get; set; }

    }
}
