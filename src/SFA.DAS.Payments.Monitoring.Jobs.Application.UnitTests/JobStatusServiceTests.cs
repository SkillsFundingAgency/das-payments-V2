using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{


    [TestFixture]
    public class JobStatusServiceTests
    {
        private AutoMock mocker;
        private Dictionary<JobStepStatus, int> stepsStatuses;
        private DateTimeOffset? lastJobStepEndTime;
        private JobModel job;

        [SetUp]
        public void SetUp()
        {
            stepsStatuses = new Dictionary<JobStepStatus, int>()
            {
                {JobStepStatus.Completed, 10 }
            };
            mocker = AutoMock.GetLoose();
            lastJobStepEndTime = DateTimeOffset.UtcNow;

            var mockDataContext = mocker.Mock<IJobsDataContext>();
            mockDataContext
                .Setup(dc => dc.GetJobStepsStatus(It.IsAny<long>()))
                .Returns(Task.FromResult(stepsStatuses));
            mockDataContext
                .Setup(dc => dc.GetLastJobStepEndTime(It.IsAny<long>()))
                .Returns(Task.FromResult(lastJobStepEndTime));
            job = new JobModel
            {
                Id = 1,
                StartTime = DateTimeOffset.UtcNow.AddSeconds(-10),
                Status = JobStatus.InProgress
            };

            mockDataContext
                .Setup(dc => dc.GetJobIdFromDcJobId(It.IsAny<long>()))
                .Returns(Task.FromResult(job.Id));
        }

        [Test]
        public async Task Records_Job_Completed_If_No_Errors()
        {
            var service = mocker.Create<JobStatusService>();
            await service.UpdateStatus(job ,default(CancellationToken));
            mocker.Mock<IJobsDataContext>()
                .Verify(dc => dc.UpdateJob(It.Is<JobModel>(j => j.Id == job.Id && j.Status == JobStatus.Completed),It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Test]
        public async Task Records_Job_Completed_With_Errors_If_Some_Steps_Failed()
        {
            stepsStatuses.Add(JobStepStatus.Failed, 1);
            var service = mocker.Create<JobStatusService>();
            await service.UpdateStatus(job, default(CancellationToken));
            mocker.Mock<IJobsDataContext>()
                .Verify(dc => dc.UpdateJob(It.Is<JobModel>(j => j.Id == job.Id && j.Status == JobStatus.CompletedWithErrors), It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Test]
        public async Task Does_Not_Save_Job_Status_If_Steps_Are_Still_Queued()
        {
            stepsStatuses.Add(JobStepStatus.Queued, 1);
            var service = mocker.Create<JobStatusService>();
            await service.UpdateStatus(job, default(CancellationToken));
            mocker.Mock<IJobsDataContext>()
                .Verify(dc => dc.SaveJobStatus(It.IsAny<long>(),
                        It.IsAny<JobStatus>(),
                        It.IsAny<DateTimeOffset>()),
                    Times.Never);
        }

        [Test]
        public async Task Does_Not_Save_Job_Status_If_Steps_Are_Still_Processing()
        {
            stepsStatuses.Add(JobStepStatus.Processing, 1);
            var service = mocker.Create<JobStatusService>();
            await service.UpdateStatus(job, default(CancellationToken));
            mocker.Mock<IJobsDataContext>()
                .Verify(dc => dc.SaveJobStatus(It.IsAny<long>(),
                        It.IsAny<JobStatus>(),
                        It.IsAny<DateTimeOffset>()),
                    Times.Never);
        }
    }
}