using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Model.Core;
using System;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class IlrSubmittedEvent : IPaymentsMessage
    {
        public long JobId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long Ukprn { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public CalendarPeriod CollectionPeriod { get; set; }
    }
}
