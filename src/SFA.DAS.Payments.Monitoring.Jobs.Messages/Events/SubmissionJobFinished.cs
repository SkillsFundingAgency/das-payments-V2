using System;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Monitoring.Jobs.Messages.Events
{
    public class SubmissionJobFinished: IEvent
    {
        public Guid EventId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long JobId { get; set; }
        public long Ukprn { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }
        public bool Succeeded { get; set; }

        public SubmissionJobFinished()
        {
            EventTime = DateTimeOffset.UtcNow;
            EventId = Guid.NewGuid();
        }
    }
}