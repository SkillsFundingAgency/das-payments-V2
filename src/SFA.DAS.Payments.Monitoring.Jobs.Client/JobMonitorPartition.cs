namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public static class JobMonitorPartition
    {
        public static long PartitionNameForJob(long jobId) => jobId % 50;
    }
}
