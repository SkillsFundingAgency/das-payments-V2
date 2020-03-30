using System;
using SFA.DAS.Payments.Messages.Core.Commands;

namespace SFA.DAS.Payments.Monitoring.Metrics.Messages.Commands
{
    public class GenerateSubmissionSummary : IPaymentsCommand
    {
        public long Ukprn { get; set; }
        public long JobId { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public Guid CommandId { get; set; }
        public DateTimeOffset RequestTime { get; set; }

        public GenerateSubmissionSummary()
        {
            CommandId = Guid.NewGuid();
            RequestTime = DateTimeOffset.UtcNow;
        }
    }
}
