using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Messages.Events
{
    public class StartedProcessingProviderEarningsEvent : JobStatusEvent
    {
        public DateTime IlrSubmissionTime { get; set; }
        public long Ukprn { get; set; }
        public short CollectionYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public List<(DateTimeOffset StartTime, Guid EventId)> SubEventIds { get; set; }

        public StartedProcessingProviderEarningsEvent()
        {
            StartTime = DateTimeOffset.UtcNow;
            SubEventIds = new List<(DateTimeOffset StartTime, Guid EventId)>();
        }
    }
}