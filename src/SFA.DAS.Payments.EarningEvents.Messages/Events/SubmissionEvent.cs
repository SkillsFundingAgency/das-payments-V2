using System;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public abstract class SubmissionEvent : IPaymentsMessage
    {
        public long JobId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long Ukprn { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }
    }
}