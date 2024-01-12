using SFA.DAS.Payments.Messages.Common;
using SFA.DAS.Payments.Messages.Common.Commands;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.FundingSource.Messages.Internal.Commands
{
    public class ProcessLevyPaymentsOnMonthEndCommand : PaymentsCommand, IMonitoredMessage
    {
        public CollectionPeriod CollectionPeriod { get; set; }

        public long AccountId { get; set; }

    }
}
