using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Messages.Commands
{
    public class RecordJobMessageProcessingStatus : JobStatusMessage
    {
        public DateTimeOffset EndTime { get; set; }
        public Guid Id { get; set; }
        public bool Succeeded { get; set; }
        public List<(DateTimeOffset StartTime, Guid EventId)> GeneratedEvents { get; set; }

        public RecordJobMessageProcessingStatus()
        {
            GeneratedEvents = new List<(DateTimeOffset StartTime, Guid EventId)>();
        }
    }
}