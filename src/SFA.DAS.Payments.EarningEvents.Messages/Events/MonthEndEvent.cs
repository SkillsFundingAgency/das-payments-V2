using SFA.DAS.Payments.Model.Core;
using System;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class MonthEndEvent
    {
        CalendarPeriod CollectionPeriod { get; }
        public DateTimeOffset EventTime { get; set; }
    }
}
