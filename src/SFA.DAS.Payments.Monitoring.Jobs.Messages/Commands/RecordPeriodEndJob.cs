using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands
{
    public abstract class RecordPeriodEndJob : JobsCommand
    {
        public short CollectionYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public List<GeneratedMessage> GeneratedMessages { get; set; }

        protected RecordPeriodEndJob()
        {
            StartTime = DateTimeOffset.UtcNow;
            GeneratedMessages = new List<GeneratedMessage>();
        }
    }
}