using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.JobContextMessageHandling.JobStatus;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.PeriodEnd.Application.UnitTests.JobStatusServiceTests
{
    [TestFixture]
    public class WaitForJobsToFinishTimeoutTests
    {
        private AutoMock mocker;
        private JobModel job;
        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            job = new JobModel
            {
                Id = 123,
                Ukprn = 4321,
                AcademicYear = 1920,
                CollectionPeriod = 10,
                DcJobId = 456,
                JobType = JobType.PeriodEndStartJob,
                StartTime = DateTimeOffset.UtcNow,
                LearnerCount = 100,
                Status = JobStatus.InProgress
            };
            mocker.Mock<IJobsDataContext>()
                .Setup(dc => dc.GetJobByDcJobId(It.IsAny<long>()))
                .ReturnsAsync(job);
            mocker.Mock<IJobStatusConfiguration>()
                .Setup(cfg => cfg.TimeToPauseBetweenChecks).Returns(TimeSpan.FromMilliseconds(500));
            mocker.Mock<IJobStatusConfiguration>()
                .Setup(cfg => cfg.TimeToWaitForJobToComplete).Returns(TimeSpan.FromSeconds(1));
            mocker.Mock<IJobStatusConfiguration>()
                .Setup(cfg => cfg.TimeToWaitForPeriodEndRunJobToComplete).Returns(TimeSpan.FromSeconds(1));
        }

        [Test]
        public async Task Returns_True_If_Job_Completed()
        {
            job.Status = JobStatus.Completed;
            var service = mocker.Create<JobStatusService>();
            var finished = await service.WaitForJobToFinish(1, CancellationToken.None).ConfigureAwait(false);
            finished.Should().BeTrue();
        }

        [Test]
        public async Task Returns_True_If_Job_Completed_For_Period_End_Run_Job()
        {
            job.Status = JobStatus.Completed;
            var service = mocker.Create<JobStatusService>();
            var finished = await service.WaitForJobToFinish(1, CancellationToken.None, true).ConfigureAwait(false);
            finished.Should().BeTrue();
        }

        [TestCase(1, 0, true, false)]
        [TestCase(0, 1, true, true)]
        [TestCase(0, 1, false, false)]
        [TestCase(1, 0, false, true)]
        public async Task Returns_Correctly_Based_On_Specific_Timeouts(int defaultTimeout, int periodEndRunTimeout, bool isPeriodEndJob, bool expectedResult)
        {
            mocker.Mock<IJobStatusConfiguration>()
                .Setup(cfg => cfg.TimeToWaitForJobToComplete).Returns(TimeSpan.FromSeconds(defaultTimeout));
            mocker.Mock<IJobStatusConfiguration>()
                .Setup(cfg => cfg.TimeToWaitForPeriodEndRunJobToComplete).Returns(TimeSpan.FromSeconds(periodEndRunTimeout));
            if (isPeriodEndJob)
                job.JobType = JobType.PeriodEndRunJob;
            job.Status = JobStatus.Completed;
            var service = mocker.Create<JobStatusService>();
            var finished = await service.WaitForJobToFinish(1, CancellationToken.None, isPeriodEndJob).ConfigureAwait(false);
            finished.Should().Be(expectedResult);
        }
    }
}