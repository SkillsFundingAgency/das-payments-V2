using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class JobStatsServiceTests
    {
        private AutoMock mocker;
        private Dictionary<JobStepStatus, int> stepsStatuses;
        private DateTimeOffset lastJobStepEndTime;
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

            var mockDataContext = mocker.Mock<IJobStatusDataContext>();
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
                Status = Data.Model.JobStatus.InProgress
            };

            mockDataContext
                .Setup(dc => dc.GetJobIdFromDcJobId(It.IsAny<long>()))
                .Returns(Task.FromResult(job.Id));
        }

        [Test]
        public async Task Records_Job_Completed_If_No_Errors()
        {
            var service = mocker.Create<JobStatusService>();
            await service.JobStepsCompleted(job.Id);
            mocker.Mock<IJobStatusDataContext>()
                .Verify(dc => dc.SaveJobStatus(It.Is<long>(id => id == job.Id),
                        It.Is<Data.Model.JobStatus>(status => status == Data.Model.JobStatus.Completed),
                        It.Is<DateTimeOffset>(time => time == lastJobStepEndTime)),
                    Times.Once);
        }

        [Test]
        public async Task Records_Job_Completed_With_Errors_If_Some_Steps_Failed()
        {
            stepsStatuses.Add(JobStepStatus.Failed, 1);
            var service = mocker.Create<JobStatusService>();
            await service.JobStepsCompleted(job.Id);
            mocker.Mock<IJobStatusDataContext>()
                .Verify(dc => dc.SaveJobStatus(It.Is<long>(id => id == job.Id),
                        It.Is<Data.Model.JobStatus>(status => status == Data.Model.JobStatus.CompletedWithErrors),
                        It.Is<DateTimeOffset>(time => time == lastJobStepEndTime)),
                    Times.Once);
        }

        [Test]
        public async Task Does_Not_Save_Job_Status_If_Steps_Are_Still_Queued()
        {
            stepsStatuses.Add(JobStepStatus.Queued, 1);
            var service = mocker.Create<JobStatusService>();
            await service.JobStepsCompleted(job.Id);
            mocker.Mock<IJobStatusDataContext>()
                .Verify(dc => dc.SaveJobStatus(It.IsAny<long>(),
                        It.IsAny<Data.Model.JobStatus>(),
                        It.IsAny<DateTimeOffset>()),
                    Times.Never);
        }

        [Test]
        public async Task Does_Not_Save_Job_Status_If_Steps_Are_Still_Processing()
        {
            stepsStatuses.Add(JobStepStatus.Processing, 1);
            var service = mocker.Create<JobStatusService>();
            await service.JobStepsCompleted(job.Id);
            mocker.Mock<IJobStatusDataContext>()
                .Verify(dc => dc.SaveJobStatus(It.IsAny<long>(),
                        It.IsAny<Data.Model.JobStatus>(),
                        It.IsAny<DateTimeOffset>()),
                    Times.Never);
        }
    }
}