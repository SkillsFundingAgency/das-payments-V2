using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class PeriodEndStartJobStatusServiceTests
    {
        private AutoMock mocker;
        private JobModel job;
        private bool doesPeriodEndSummaryExistForJob;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            doesPeriodEndSummaryExistForJob = true;

            job = new JobModel
            {
                DcJobId = 3459876,
                StartTime = DateTimeOffset.Now.AddHours(-1),
                Id = 1146
            };

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.DoesSubmissionSummaryExistForJob(job.DcJobId))
                .ReturnsAsync(doesPeriodEndSummaryExistForJob);
        }

        [TestCase(JobStatus.TimedOut)]
        [TestCase(JobStatus.DcTasksFailed)]
        public async Task ReturnsCorrectly_WhenTimeoutsPresent(JobStatus status)
        {
            var outstandingJobs = new List<OutstandingJobResult>
            {
                new OutstandingJobResult
                {
                    JobStatus = status,
                    EndTime = DateTimeOffset.Now
                }
            };

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(job.DcJobId, job.StartTime, It.IsAny<CancellationToken>()))
                .ReturnsAsync(outstandingJobs);

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.PerformAdditionalJobChecks(job, CancellationToken.None);
            result.Should().Be((true, JobStatus.CompletedWithErrors, outstandingJobs.Single().EndTime));
        }

        [Test]
        public async Task ReturnsCorrectly_WhenOutstandingJobWithInProgressStatusExists()
        {
            var outstandingJobs = new List<OutstandingJobResult>
            {
                new OutstandingJobResult
                {
                    JobStatus = JobStatus.InProgress,
                    DcJobSucceeded = false
                }
            };

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(job.DcJobId, job.StartTime, It.IsAny<CancellationToken>()))
                .ReturnsAsync(outstandingJobs);

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.PerformAdditionalJobChecks(job, CancellationToken.None);
            result.Should().Be((false, null, null));
        }

        [Test]
        public async Task ReturnsCorrectly_WhenOutstandingJobNullDcJobSucceededExists()
        {
            var outstandingJobs = new List<OutstandingJobResult>
            {
                new OutstandingJobResult
                {
                    DcJobSucceeded = null
                }
            };

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(job.DcJobId, job.StartTime, It.IsAny<CancellationToken>()))
                .ReturnsAsync(outstandingJobs);

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.PerformAdditionalJobChecks(job, CancellationToken.None);
            result.Should().Be((false, null, null));
        }

        [Test]
        public async Task ReturnsCorrectly_WhenPeriodEndSummaryMetricsDoNotExist()
        {
            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(job.DcJobId, job.StartTime, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OutstandingJobResult>());

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.DoesSubmissionSummaryExistForJob(job.DcJobId))
                .ReturnsAsync(false);

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.PerformAdditionalJobChecks(job, CancellationToken.None);
            result.Should().Be((false, null, null));
        }

        [Test]
        public async Task ReturnsCorrectly_WhenAllChecksPass()
        {
            var expectedEndTime = DateTimeOffset.Now;
            var outstandingJobs = new List<OutstandingJobResult>
            {
                new OutstandingJobResult
                {
                    JobStatus = JobStatus.Completed,
                    EndTime = expectedEndTime,
                    DcJobSucceeded = true
                },
                new OutstandingJobResult
                {
                    JobStatus = JobStatus.Completed,
                    EndTime = expectedEndTime.AddMinutes(-1),
                    DcJobSucceeded = false
                }
            };

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(job.DcJobId, job.StartTime, It.IsAny<CancellationToken>()))
                .ReturnsAsync(outstandingJobs);

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.DoesSubmissionSummaryExistForJob(job.DcJobId))
                .ReturnsAsync(true);

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.PerformAdditionalJobChecks(job, CancellationToken.None);
            result.Should().Be((true, null, expectedEndTime));
        }
    }
}