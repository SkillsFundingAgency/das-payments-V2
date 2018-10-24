using SFA.DAS.Payments.Model.Core;
using System;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class MonthEndEvent: IPaymentsMessage
    {
        public long JobId { get; set; }
        CalendarPeriod CollectionPeriod { get; set; }
        public DateTimeOffset EventTime { get; set; }
    }
}
