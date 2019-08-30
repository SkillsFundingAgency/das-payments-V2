using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class PeriodEndEventModel
    {
        public Guid EventId { get; set; }
        public long JobId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public short AcademicYear { get; set; }
        public byte Period { get; set; }
        public string EventType { get; set; }
    }
}
