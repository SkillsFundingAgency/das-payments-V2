using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Messages.Events
{
    public class ProcessedJobMessage : JobStatusEvent
    {
        public DateTimeOffset EndTime { get; set; }
        public Guid Id { get; set; }
        public bool Succeeded { get; set; }
        public List<(DateTimeOffset StartTime, Guid EventId)> GeneratedEvents { get; set; }

        public ProcessedJobMessage()
        {
            GeneratedEvents = new List<(DateTimeOffset StartTime, Guid EventId)>();
        }
    }
}