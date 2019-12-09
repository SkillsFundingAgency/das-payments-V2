using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands
{
    public abstract class RecordEarningsJobStatus : JobsCommand
    {
        public long Ukprn { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }
    }
}