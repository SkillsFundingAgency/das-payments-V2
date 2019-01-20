using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ProviderPayments.Messages.Commands
{
    public class PerformMonthEndProcessingCommand : PaymentsCommand
    {
        public CollectionPeriod CollectionPeriod { get; set; }
    }
}
