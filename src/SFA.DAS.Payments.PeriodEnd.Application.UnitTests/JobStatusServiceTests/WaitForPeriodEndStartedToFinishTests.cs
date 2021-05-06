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
    public class WaitForPeriodEndStartedToFinishTests
    {
        private AutoMock mocker;
        private JobModel job;
        private JobStatusService sut;
        
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
                .Setup(dc => dc.GetJobByDcJobId(456))
                .ReturnsAsync(job);
            mocker.Mock<IJobStatusConfiguration>()
                .Setup(cfg => cfg.TimeToPauseBetweenChecks).Returns(TimeSpan.FromMilliseconds(10));
            mocker.Mock<IJobStatusConfiguration>()
                .Setup(cfg => cfg.TimeToWaitForJobToComplete).Returns(TimeSpan.FromSeconds(1));

            sut = mocker.Create<JobStatusService>();
        }
        
        [Test]
        public async Task Returns_True_If_Job_Completed_Successfully()
        {
            job.Status = JobStatus.Completed;
            var actual = await sut.WaitForDcJobToFinish(456, CancellationToken.None);
            actual.Should().BeTrue();
        }


        [Test]
        public async Task Returns_False_If_Job_Completed_With_Errors()
        {
            job.Status = JobStatus.CompletedWithErrors;
            var actual = await sut.WaitForDcJobToFinish(456, CancellationToken.None);
            actual.Should().BeFalse();
        }

        [Test]
        public async Task Returns_False_If_Job_TimesOut()
        {
            job.Status = JobStatus.InProgress;
            var actual = await sut.WaitForDcJobToFinish(1, CancellationToken.None);
            actual.Should().BeFalse();
        }
    }
}
