using System;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Messages.Commands
{
    public class RecordNewJobCommand
    {

    }

    public class RecordProviderEarningsReceivedCommand : RecordNewJobCommand
    {
        public long JobId { get; set; }
        public DateTime IlrSubmissionTime { get; set; }
        public long Ukprn { get; set; }
        public string CollectionYear { get; set; }
        public byte CollectionPeriod { get; set; }
    }
}