using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
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
        private List<long?> doSubmissionSummariesExistForJob;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            doSubmissionSummariesExistForJob = new List<long?> { 1 };

            job = new JobModel
            {
                DcJobId = 3459876,
                StartTime = DateTimeOffset.Now.AddHours(-1),
                Id = 1146
            };

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.DoSubmissionSummariesExistForJobs(It.IsAny<List<OutstandingJobResult>>()))
                .Returns(doSubmissionSummariesExistForJob);


            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetAverageJobCompletionTimesForInProgressJobs(It.IsAny<List<long?>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InProgressJobAverageJobCompletionTime>
                {
                    new InProgressJobAverageJobCompletionTime
                    {
                        JobId = 1146,
                        AverageJobCompletionTime = 2000000
                    }
                });
        }

        [TestCase(JobStatus.TimedOut)]
        [TestCase(JobStatus.DcTasksFailed)]
        public async Task Returns_CompletedWithErrors_WhenTimeoutsPresent(JobStatus status)
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
                .Setup(x => x.GetOutstandingOrTimedOutJobs(job, It.IsAny<CancellationToken>()))
                .ReturnsAsync(outstandingJobs);

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.PerformAdditionalJobChecks(job, CancellationToken.None);
            result.Should().Be((true, JobStatus.CompletedWithErrors, outstandingJobs.Single().EndTime));
        }

        [Test]
        public async Task Returns_InProgress_WhenOutstandingJobWithInProgressStatusExists()
        {
            var outstandingJobs = new List<OutstandingJobResult>
            {
                new OutstandingJobResult
                {
                    DcJobId = 1,
                    JobStatus = JobStatus.InProgress,
                    DcJobSucceeded = false
                }
            };

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(job, It.IsAny<CancellationToken>()))
                .ReturnsAsync(outstandingJobs);

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.PerformAdditionalJobChecks(job, CancellationToken.None);

            result.Should().Be((false, null, null));

            mocker.Mock<ITelemetry>().Verify(t => t.TrackEvent(
                "PeriodEndStart Job Status Update",
                It.Is<Dictionary<string, string>>(d => 
                    d.Contains(new KeyValuePair<string, string>("InProgressJobsCount", "1"))),
                It.IsAny<Dictionary<string, double>>()));
        }

        [Test]
        public async Task Returns_InProgress_WhenOutstandingJobNullDcJobSucceededExists()
        {
            var outstandingJobs = new List<OutstandingJobResult>
            {
                new OutstandingJobResult
                {
                    DcJobId = 1,
                    JobStatus = JobStatus.Completed,
                    DcJobSucceeded = null
                }
            };

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(job, It.IsAny<CancellationToken>()))
                .ReturnsAsync(outstandingJobs);

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.PerformAdditionalJobChecks(job, CancellationToken.None);
            
            result.Should().Be((false, null, null));

            mocker.Mock<ITelemetry>().Verify(t => t.TrackEvent(
                "PeriodEndStart Job Status Update",
                It.Is<Dictionary<string, string>>(d => 
                    d.Contains(new KeyValuePair<string, string>("InProgressJobsCount", "1"))),
                It.IsAny<Dictionary<string, double>>()));
        }

        [Test]
        public async Task Returns_InProgress_WhenSubmissionSummariesDoNotExist()
        {
            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(job, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OutstandingJobResult>());

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.DoSubmissionSummariesExistForJobs(It.IsAny<List<OutstandingJobResult>>()))
                .Returns(new List<long?> { 1, 2 });

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.PerformAdditionalJobChecks(job, CancellationToken.None);

            result.Should().Be((false, null, null));

            mocker.Mock<ITelemetry>().Verify(t => t.TrackEvent(
                "PeriodEndStart Job Status Update",
                It.Is<Dictionary<string, string>>(d => 
                    d.Contains(new KeyValuePair<string, string>("jobsWithoutSubmissionSummariesCount", "2")) &&
                    d.Contains(new KeyValuePair<string, string>("jobsWithoutSubmissionSummaries", "1, 2"))),
                It.IsAny<Dictionary<string, double>>()));
        }

        [Test]
        public async Task Returns_Completed_WhenAllChecksPass()
        {
            var expectedEndTime = DateTimeOffset.UtcNow;
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
                .Setup(x => x.GetOutstandingOrTimedOutJobs(job, It.IsAny<CancellationToken>()))
                .ReturnsAsync(outstandingJobs);

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.DoSubmissionSummariesExistForJobs(It.IsAny<List<OutstandingJobResult>>()))
                .Returns(new List<long?>());

            var service = mocker.Create<PeriodEndStartJobStatusService>();

            (bool IsComplete, JobStatus? OverriddenJobStatus, DateTimeOffset? completionTime) result = await service.PerformAdditionalJobChecks(job, CancellationToken.None);

            result.IsComplete.Should().BeTrue();
            result.OverriddenJobStatus.Should().Be(null);
            result.completionTime.Should().BeCloseTo(expectedEndTime, 500);

            mocker.Mock<ITelemetry>().Verify(t => 
                t.TrackEvent("PeriodEndStart Job Status Update", It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, double>>()), 
                Times.Never);

        }
    }
}