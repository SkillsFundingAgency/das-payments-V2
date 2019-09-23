using NUnit.Framework;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client.UnitTests
{
    [TestFixture]
    public class PartitionNameTests
    {
        [TestCase(1, 0, 1)]
        [TestCase(2, 0, 2)]
        [TestCase(3, 0, 3)]
        [TestCase(4, 0, 4)]
        [TestCase(5, 0, 5)]
        [TestCase(6, 0, 6)]
        [TestCase(7, 0, 7)]
        [TestCase(8, 0, 8)]
        [TestCase(9, 0, 9)]
        [TestCase(10, 0, 10)]
        [TestCase(11, 0, 11)]
        [TestCase(12, 0, 12)]
        [TestCase(13, 0, 13)]
        [TestCase(14, 0, 14)]
        [TestCase(15, 0, 15)]
        [TestCase(16, 0, 16)]
        [TestCase(17, 0, 17)]
        [TestCase(18, 0, 18)]
        [TestCase(19, 0, 19)]
        [TestCase(20, 0, 0)]
        [TestCase(61, 0, 1)]
        [TestCase(62, 0, 2)]
        [TestCase(63, 0, 3)]
        [TestCase(64, 0, 4)]
        [TestCase(65, 0, 5)]
        [TestCase(66, 0, 6)]
        [TestCase(67, 0, 7)]
        [TestCase(68, 0, 8)]
        [TestCase(69, 0, 9)]
        [TestCase(70, 0, 10)]
        [TestCase(71, 0, 11)]
        [TestCase(72, 0, 12)]
        [TestCase(73, 0, 13)]
        [TestCase(74, 0, 14)]
        [TestCase(75, 0, 15)]
        [TestCase(76, 0, 16)]
        [TestCase(77, 0, 17)]
        [TestCase(78, 0, 18)]
        [TestCase(79, 0, 19)]
        [TestCase(80, 0, 0)]
        [TestCase(1, 10003915, 10003915)]
        [TestCase(2, 10033440, 10033440)]
        [TestCase(3, 10003161, 10003161)]
        [TestCase(4, 10012467, 10012467)]
        [TestCase(5, 10000446, 10000446)]
        public void Naming(long jobId, long ukprn, long name)
        {
            var actual = new JobMonitorPartition().PartitionNameForJob(jobId, ukprn);
            Assert.That(actual, Is.EqualTo(name));
        }
    }
}