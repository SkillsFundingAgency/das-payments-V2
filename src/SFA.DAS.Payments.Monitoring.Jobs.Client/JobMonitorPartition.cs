using System.Linq;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public class JobMonitorPartition
    {
        public static long[] InvidivualNodes = new long[]
        {
            10003915,
            10033440,
            10003161,
            10012467,
            10000446,
        };

        public long PartitionNameForJob(long jobId, long Ukprn)
        {
            return InvidivualNodes.Contains(Ukprn) ? Ukprn : Partition(jobId);
        }

        private static long Partition(long jobId) => (jobId % 20);
    }
}
