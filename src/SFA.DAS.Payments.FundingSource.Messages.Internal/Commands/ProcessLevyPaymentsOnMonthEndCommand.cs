using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.FundingSource.Messages.Internal.Commands
{
    public class ProcessLevyPaymentsOnMonthEndCommand : PaymentsCommand
    {
        public CollectionPeriod CollectionPeriod { get; set; }

        public long EmployerAccountId { get; set; }
    }
}
