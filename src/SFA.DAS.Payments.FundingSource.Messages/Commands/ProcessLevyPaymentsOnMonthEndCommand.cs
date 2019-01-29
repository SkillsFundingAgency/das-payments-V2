using System.Collections.Generic;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.FundingSource.Messages.Commands
{
    public class ProcessLevyPaymentsOnMonthEndCommand : PaymentsCommand
    {
        public CollectionPeriod CollectionPeriod { get; set; }

        public IReadOnlyCollection<long> EmployerIds { get; set; }
    }
}
