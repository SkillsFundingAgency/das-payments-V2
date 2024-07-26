namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData
{
    public class CommitmentStatsDto
    {
        public long Approved { get; set; }
        public long Stopped { get; set; }
        public long Paused { get; set; }
    }
}