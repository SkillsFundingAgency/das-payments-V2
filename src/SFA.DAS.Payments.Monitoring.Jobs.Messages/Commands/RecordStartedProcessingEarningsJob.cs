using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands
{
    public class RecordStartedProcessingEarningsJob : JobsCommand
    {
        public DateTime IlrSubmissionTime { get; set; }
        public long Ukprn { get; set; }
        public short CollectionYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public List<GeneratedMessage> GeneratedMessages { get; set; }
        public int LearnerCount { get; set; }
        public RecordStartedProcessingEarningsJob()
        {
            StartTime = DateTimeOffset.UtcNow;
            GeneratedMessages = new List<GeneratedMessage>();
        }
    }
}