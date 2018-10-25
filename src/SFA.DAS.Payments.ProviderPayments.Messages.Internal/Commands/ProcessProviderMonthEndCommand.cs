using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands
{
    public class ProcessProviderMonthEndCommand:IPaymentsMessage
    {
        public long Ukprn { get; set; }
        public long JobId { get; set; }
        public CalendarPeriod CollectionPeriod { get; set; }

    }
}
