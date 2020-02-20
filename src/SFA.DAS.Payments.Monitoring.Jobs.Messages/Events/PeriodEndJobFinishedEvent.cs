using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Monitoring.Jobs.Messages.Events
{
    public class PeriodEndJobFinishedEvent :IEvent
    {
        public Guid EventId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public long JobId { get; set; }
        public PeriodEndJobFinishedEvent()
        {
            EventTime = DateTimeOffset.UtcNow;
            EventId = Guid.NewGuid();
        }
    }
}