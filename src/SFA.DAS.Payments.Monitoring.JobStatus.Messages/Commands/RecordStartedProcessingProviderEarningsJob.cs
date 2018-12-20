using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Messages.Commands
{
    public class RecordStartedProcessingProviderEarningsJob : JobStatusMessage
    {
        public DateTime IlrSubmissionTime { get; set; }
        public long Ukprn { get; set; }
        public short CollectionYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public List<(DateTimeOffset StartTime, Guid EventId)> SubEventIds { get; set; }

        public RecordStartedProcessingProviderEarningsJob()
        {
            StartTime = DateTimeOffset.UtcNow;
            SubEventIds = new List<(DateTimeOffset StartTime, Guid EventId)>();
        }
    }
}