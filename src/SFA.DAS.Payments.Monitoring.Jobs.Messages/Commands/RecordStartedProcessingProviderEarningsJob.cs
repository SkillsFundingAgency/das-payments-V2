using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands
{
    public class RecordStartedProcessingProviderEarningsJob : JobsCommand
    {
        public DateTime IlrSubmissionTime { get; set; }
        public long Ukprn { get; set; }
        public short CollectionYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public List<GeneratedMessage> GeneratedMessages { get; set; }

        public RecordStartedProcessingProviderEarningsJob()
        {
            StartTime = DateTimeOffset.UtcNow;
            GeneratedMessages = new List<GeneratedMessage>();
        }
    }
}