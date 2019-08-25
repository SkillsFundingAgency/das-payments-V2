using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class EarningsJobServiceTests
    {
        private AutoMock mocker;
        private JobStepModel jobStep;
        private RecordJobMessageProcessingStatus jobMessageStatus;
        private List<JobStepModel> jobSteps;

        [SetUp]
        public void SetUp()
        {
            jobMessageStatus = new RecordJobMessageProcessingStatus
            {
                JobId = 1,
                MessageName = "Message1",
                Id = Guid.NewGuid(),
                Succeeded = true,
                EndTime = DateTimeOffset.UtcNow,
                GeneratedMessages = new List<GeneratedMessage>()
            };
            jobStep = new JobStepModel
            {
                JobId = 21,
                StartTime = DateTimeOffset.UtcNow.AddSeconds(-10),
                MessageName = "Message1",
                MessageId = jobMessageStatus.Id,
                Id = 2,
                Status = JobStepStatus.Queued,
            };
            mocker = AutoMock.GetLoose();
            var mockDataContext = mocker.Mock<IJobsDataContext>();
            mockDataContext.Setup(x => x.GetJobIdFromDcJobId(It.IsAny<long>()))
                .Returns(Task.FromResult<long>(99));
            mockDataContext.Setup(x => x.GetJobByDcJobId(It.IsAny<long>()))
                .Returns(Task.FromResult<JobModel>(new JobModel { Id = jobStep.JobId, DcJobId = jobMessageStatus.JobId }));
            jobSteps = new List<JobStepModel> { jobStep };
            mockDataContext
                .Setup(dc => dc.GetJobSteps(It.IsAny<List<Guid>>()))
                .Returns(Task.FromResult<List<JobStepModel>>(jobSteps));
            mockDataContext.Setup(dc => dc.GetJobIdFromDcJobId(It.IsAny<long>()))
                .Returns(Task.FromResult<long>(1));
            object job = new JobModel { Id = jobStep.JobId, DcJobId = jobMessageStatus.JobId };
            mocker.Mock<IMemoryCache>()
                .Setup(cache => cache.TryGetValue(It.IsAny<string>(), out job))
                .Returns(true);
        }

        [Test]
        public async Task Stores_New_Jobs()
        {
            var jobStarted = new RecordEarningsJob
            {
                CollectionPeriod = 1,
                CollectionYear = 1819,
                JobId = 1,
                Ukprn = 9999,
                IlrSubmissionTime = DateTime.UtcNow.AddMinutes(-20),
                StartTime = DateTimeOffset.UtcNow,
            };
            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetJobByDcJobId(It.Is<long>(id => id == 1)))
                .ReturnsAsync((JobModel)null);

            var service = mocker.Create<EarningsJobService>();
            await service.JobStarted(jobStarted);
            mocker.Mock<IJobsDataContext>()
                .Verify(dc => dc.SaveNewJob(
                    It.Is<JobModel>(job =>
                        job.StartTime == jobStarted.StartTime
                        && job.JobType == JobType.EarningsJob
                        && job.Status == JobStatus.InProgress && job.DcJobId == jobStarted.JobId
                        && job.CollectionPeriod == jobStarted.CollectionPeriod
                        && job.AcademicYear == jobStarted.CollectionYear
                        && job.IlrSubmissionTime == jobStarted.IlrSubmissionTime
                        && job.Ukprn == jobStarted.Ukprn),
                    It.IsAny<List<JobStepModel>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task RecordStartedProcessingEarningsJob_Updates_Existing_Job()
        {
            var storedJob = new JobModel();
            mocker.Mock<IJobsDataContext>()
                .Setup(dc => dc.GetJobByDcJobId(It.IsAny<long>()))
                .ReturnsAsync(storedJob);

            var generatedMessage = new GeneratedMessage
            {

                StartTime = DateTimeOffset.UtcNow,
                MessageId = Guid.NewGuid(),
                MessageName = "MessageA",
            };
            var jobStarted = new RecordEarningsJob
            {
                CollectionPeriod = 1,
                CollectionYear = 1819,
                JobId = 1,
                Ukprn = 9999,
                IlrSubmissionTime = DateTime.UtcNow.AddMinutes(-20),
                StartTime = DateTimeOffset.UtcNow,
                GeneratedMessages = new List<GeneratedMessage> { generatedMessage }
            };
            var service = mocker.Create<EarningsJobService>();
            await service.JobStarted(jobStarted);
            mocker.Mock<IJobsDataContext>()
                .Verify(dc => dc.SaveJobSteps(It.Is<List<JobStepModel>>(list =>
                    list.Any(item =>
                        item.MessageId == generatedMessage.MessageId &&
                        item.Status == JobStepStatus.Queued &&
                        item.StartTime == generatedMessage.StartTime))), Times.Once);
        }

        private async Task JobStepCompleted()
        {
            var service = mocker.Create<JobStepService>();
            await service.JobStepCompleted(jobMessageStatus);
        }

        [Test]
        public async Task Records_Status_Of_Completed_JobStep()
        {
            await JobStepCompleted();
            mocker.Mock<IJobsDataContext>()
                .Verify(dc => dc.SaveJobSteps(It.Is<List<JobStepModel>>(list =>
                    list.Any(item =>
                        item.Id == jobStep.Id &&
                        item.MessageId == jobMessageStatus.Id &&
                        item.Status == JobStepStatus.Completed &&
                        item.EndTime == jobMessageStatus.EndTime))), Times.Once);
        }

        [Test]
        public async Task Creates_New_Completed_Step_Model_If_Not_Found()
        {
            jobSteps.Clear();
            await JobStepCompleted();
            mocker.Mock<IJobsDataContext>()
                .Verify(dc => dc.SaveJobSteps(It.Is<List<JobStepModel>>(list =>
                    list.Any(item =>
                        item.Id == 0 &&
                        item.MessageId == jobMessageStatus.Id &&
                        item.Status == JobStepStatus.Completed &&
                        item.EndTime == jobMessageStatus.EndTime))), Times.Once);
        }

        [Test]
        public async Task Records_Status_Of_Generated_Messages()
        {
            var generatedMessage = new GeneratedMessage
            {

                StartTime = DateTimeOffset.UtcNow,
                MessageId = Guid.NewGuid(),
                MessageName = "MessageA",
            };
            jobMessageStatus.GeneratedMessages.Add(generatedMessage);
            await JobStepCompleted();
            mocker.Mock<IJobsDataContext>()
                .Verify(dc => dc.SaveJobSteps(It.Is<List<JobStepModel>>(list =>
                    list.Any(item =>
                        item.MessageId == generatedMessage.MessageId &&
                        item.Status == JobStepStatus.Queued &&
                        item.StartTime == generatedMessage.StartTime))), Times.Once);
        }

        [Test]
        public async Task Records_Start_Time_Of_Existing_Generated_Messages()
        {
            var generatedMessage = new GeneratedMessage
            {
                StartTime = DateTimeOffset.UtcNow,
                MessageId = Guid.NewGuid(),
                MessageName = "MessageA",
            };
            jobMessageStatus.GeneratedMessages.Add(generatedMessage);
            jobSteps.Add(new JobStepModel
            {
                JobId = jobStep.JobId,
                Id = 1002,
                EndTime = DateTimeOffset.UtcNow,
                Status = JobStepStatus.Completed,
                MessageId = generatedMessage.MessageId,
                MessageName = generatedMessage.MessageName,

            });
            await JobStepCompleted();
            mocker.Mock<IJobsDataContext>()
                .Verify(dc => dc.SaveJobSteps(It.Is<List<JobStepModel>>(list =>
                    list.Any(item =>
                        item.ParentMessageId == jobMessageStatus.Id &&
                        item.MessageId == generatedMessage.MessageId &&
                        item.Status == JobStepStatus.Completed &&
                        item.StartTime == generatedMessage.StartTime))), Times.Once);

        }

    }
}