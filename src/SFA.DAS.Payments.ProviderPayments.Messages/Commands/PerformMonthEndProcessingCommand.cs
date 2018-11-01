using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ProviderPayments.Messages.Commands
{
    public class PerformMonthEndProcessingCommand : IPaymentsMessage
    {
        public long JobId { get; set; }
        public CalendarPeriod CollectionPeriod { get; set; }
    }
}
