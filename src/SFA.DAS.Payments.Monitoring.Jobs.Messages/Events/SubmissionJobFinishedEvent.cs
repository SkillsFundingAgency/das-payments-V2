using System;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Monitoring.Jobs.Messages.Events
{
    public abstract class SubmissionJobFinishedEvent: IEvent, ISubmissionJobFinishedEvent
    {
        public Guid EventId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long JobId { get; set; }
        public long Ukprn { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }

        protected SubmissionJobFinishedEvent()
        {
            EventTime = DateTimeOffset.UtcNow;
            EventId = Guid.NewGuid();
        }
    }

    public interface ISubmissionJobFinishedEvent
    {
        Guid EventId { get; set; }
        DateTimeOffset EventTime { get; set; }
        long JobId { get; set; }
        long Ukprn { get; set; }
        DateTime IlrSubmissionDateTime { get; set; }
        byte CollectionPeriod { get; set; }
        short AcademicYear { get; set; }
    }
}