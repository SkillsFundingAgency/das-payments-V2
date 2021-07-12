using System;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.MatchedLearner.Messages
{
    public abstract class SubmissionFinishedEvent : IEvent
    {
        public Guid EventId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long JobId { get; set; }
        public long Ukprn { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }

        protected SubmissionFinishedEvent()
        {
            EventTime = DateTimeOffset.UtcNow;
            EventId = Guid.NewGuid();
        }
    }
}