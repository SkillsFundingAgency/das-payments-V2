using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp;

namespace SFA.DAS.Payments.ScheduledJobs.UnitTests
{
    [TestFixture]
    public class BatchExtensionTests
    {
        [TestCase(10)]
        [TestCase(99)]
        [TestCase(100)]
        public void Given_Input_List_of_100_or_less_when_ToBatch_is_called_then_1_Batch_should_be_created(int items)
        {
            var batches = Enumerable.Range(1, items).Select(index => new SubmissionJobsToBeDeletedModel {DcJobId = index}).ToBatch(100).ToList();
            
            batches.Count.Should().Be(1);
            
            batches.ElementAt(0).JobsToBeDeleted.Length.Should().Be(items);
        }
        
        [TestCase(199, 100, 99)]
        [TestCase(200, 100, 100)]
        [TestCase(101, 100, 1)]
        public void Given_Input_List_of_200_or_less_when_ToBatch_is_called_then_2_Batch_should_be_created(int items, int itemCount1, int itemCount2)
        {
            var batches = Enumerable.Range(1, items).Select(index => new SubmissionJobsToBeDeletedModel {DcJobId = index}).ToBatch(100).ToList();
            
            batches.Count.Should().Be(2);
            
            batches.ElementAt(0).JobsToBeDeleted.Length.Should().Be(itemCount1);
            
            batches.ElementAt(1).JobsToBeDeleted.Length.Should().Be(itemCount2);
        }

        [TestCase(99, 1)]
        [TestCase(100, 1)]
        [TestCase(101, 2)]
        [TestCase(200, 2)]
        [TestCase(300, 3)]
        [TestCase(399, 4)]
        public void Given_Input_List_of_Items_When_ToBatch_is_called_then_Batches_of_100_or_less_are_returned(int items, int batchCount)
        {
            var batches = Enumerable.Range(1, items).Select(index => new SubmissionJobsToBeDeletedModel {DcJobId = index}).ToBatch(100);
            batches.Count().Should().Be(batchCount);
        }
    }
}