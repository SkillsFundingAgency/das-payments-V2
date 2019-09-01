using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class JobMessageServiceTests
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

            job = new JobModel
            {
                Id = 1,
                StartTime = DateTimeOffset.UtcNow.AddSeconds(-10),
                Status = JobStatus.InProgress
            };
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobMessages(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<JobStepModel>());
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<CancellationToken>()))
                .ReturnsAsync((jobStatus: JobStepStatus.Processing, endTime: null));
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetInProgressMessageIdentifiers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid>());
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetCompletedMessageIdentifiers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid>());
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetCompletedMessageIdentifiersHistory(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid>());
        }

        [Test]
        public async Task First_Completed_Message_Sets_Job_Status_And_EndTime()
        {
            var jobStatusMessage = new RecordJobMessageProcessingStatus
            {
                Id = Guid.NewGuid(),
                JobId = 1,
                GeneratedMessages = new List<GeneratedMessage>(),
                EndTime = DateTime.UtcNow,
                Succeeded = true
            };
            var service = mocker.Create<JobMessageService>();
            await service.RecordCompletedJobMessageStatus(jobStatusMessage);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreJobStatus(It.Is<JobStepStatus>(status => status == JobStepStatus.Completed),
                    It.Is<DateTimeOffset?>(endTime => endTime == jobStatusMessage.EndTime), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task Subsequent_Completed_Message_Updates_Job_Status_And_EndTime()
        {
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<CancellationToken>()))
                .ReturnsAsync((jobStatus: JobStepStatus.Completed, endTime: DateTimeOffset.UtcNow.AddSeconds(-1)));
            var jobStatusMessage = new RecordJobMessageProcessingStatus
            {
                Id = Guid.NewGuid(),
                JobId = 1,
                GeneratedMessages = new List<GeneratedMessage>(),
                EndTime = DateTime.UtcNow,
                Succeeded = true
            };
            var service = mocker.Create<JobMessageService>();
            await service.RecordCompletedJobMessageStatus(jobStatusMessage);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreJobStatus(It.Is<JobStepStatus>(status => status == JobStepStatus.Completed),
                    It.Is<DateTimeOffset?>(endTime => endTime == jobStatusMessage.EndTime), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task Out_Of_Sequence_Completed_Message_Do_Not_Update_Job_Status_And_EndTime()
        {
            var jobStatus = (jobStatus: JobStepStatus.Failed, endTime: DateTimeOffset.UtcNow.AddSeconds(10));
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobStatus);
            var jobStatusMessage = new RecordJobMessageProcessingStatus
            {
                Id = Guid.NewGuid(),
                JobId = 1,
                GeneratedMessages = new List<GeneratedMessage>(),
                EndTime = DateTime.UtcNow,
                Succeeded = true
            };
            var service = mocker.Create<JobMessageService>();
            await service.RecordCompletedJobMessageStatus(jobStatusMessage);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreJobStatus(It.Is<JobStepStatus>(status => status == jobStatus.jobStatus),
                    It.Is<DateTimeOffset?>(endTime => endTime == jobStatus.Item2), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task Completed_Messages_Are_Removed_From_In_Progress_List()
        {
            var completedMessages = new List<Guid>();
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetInProgressMessageIdentifiers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(completedMessages);
            var jobStatusMessage = new RecordJobMessageProcessingStatus
            {
                Id = Guid.NewGuid(),
                JobId = 1,
                GeneratedMessages = new List<GeneratedMessage>(),
                EndTime = DateTime.UtcNow,
                Succeeded = true
            };
            completedMessages.Add(jobStatusMessage.Id);
            var service = mocker.Create<JobMessageService>();
            await service.RecordCompletedJobMessageStatus(jobStatusMessage);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreInProgressMessageIdentifiers(It.Is<List<Guid>>(lst => !lst.Any()), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Completed_Messages_Are_Added_To_Completed_List_If_Started_Has_Not_Been_Received()
        {
            var inProgressMessages = new List<Guid>();
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetInProgressMessageIdentifiers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(inProgressMessages);
            var jobStatusMessage = new RecordJobMessageProcessingStatus
            {
                Id = Guid.NewGuid(),
                JobId = 1,
                GeneratedMessages = new List<GeneratedMessage>(),
                EndTime = DateTime.UtcNow,
                Succeeded = true
            };
            var service = mocker.Create<JobMessageService>();
            await service.RecordCompletedJobMessageStatus(jobStatusMessage);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreCompletedMessageIdentifiers(It.Is<List<Guid>>(lst => lst.Count == 1 && lst.All(item => item == jobStatusMessage.Id)), It.IsAny<CancellationToken>()), Times.Once);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreInProgressMessageIdentifiers(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Ignores_Branch_Messages()
        {
            //Shouldn't be getting branch level messages but just in case we do, ensure that they are ignored
            var generatedMessage = new GeneratedMessage
            {
                StartTime = DateTimeOffset.UtcNow,
                MessageId = Guid.NewGuid(),
                MessageName = "MessageA",
            };
            var branchMessage = new RecordJobMessageProcessingStatus
            {
                Id = Guid.NewGuid(),
                JobId = 1,
                GeneratedMessages = new List<GeneratedMessage> { generatedMessage }
            };
            var service = mocker.Create<JobMessageService>();
            await service.RecordCompletedJobMessageStatus(branchMessage);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreJobStatus(It.IsAny<JobStepStatus>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Records_Failure_Status()
        {
            var jobStatus = (JobStepStatus.Completed, DateTimeOffset.UtcNow.AddSeconds(10));
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobStatus);
            var jobStatusMessage = new RecordJobMessageProcessingStatus
            {
                Id = Guid.NewGuid(),
                JobId = 1,
                GeneratedMessages = new List<GeneratedMessage>(),
                EndTime = DateTime.UtcNow,
                Succeeded = false
            };
            var service = mocker.Create<JobMessageService>();
            await service.RecordCompletedJobMessageStatus(jobStatusMessage);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreJobStatus(It.Is<JobStepStatus>(status => status == JobStepStatus.Failed),
                    It.Is<DateTimeOffset?>(endTime => endTime == jobStatus.Item2), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task Removes_Failed_Messages_From_In_Progress()
        {
            var jobStatusMessage = new RecordJobMessageProcessingStatus
            {
                Id = Guid.NewGuid(),
                JobId = 1,
                GeneratedMessages = new List<GeneratedMessage>(),
                EndTime = DateTime.UtcNow,
                Succeeded = false
            };
            var inProgressMessages = new List<Guid> { jobStatusMessage.Id };
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetInProgressMessageIdentifiers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(inProgressMessages);
            var service = mocker.Create<JobMessageService>();
            await service.RecordCompletedJobMessageStatus(jobStatusMessage);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreInProgressMessageIdentifiers(It.Is<List<Guid>>(lst => !lst.Any()), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Records_Started_Processing_Job_Messages_If_Not_Already_Completed()
        {
            var jobMessage = new GeneratedMessage
            {
                MessageId = Guid.NewGuid()
            };
            var message = new RecordStartedProcessingJobMessages()
            {
                GeneratedMessages = new List<GeneratedMessage> { jobMessage }
            };
            var service = mocker.Create<JobMessageService>();
            await service.RecordStartedJobMessages(message, CancellationToken.None);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreInProgressMessageIdentifiers(It.Is<List<Guid>>(lst => lst.Count == 1 && lst.Contains(jobMessage.MessageId)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Does_Not_Store_Starting_Job_Messages_If_Already_Completed()
        {
            var jobMessage = new GeneratedMessage
            {
                MessageId = Guid.NewGuid()
            };
            var message = new RecordStartedProcessingJobMessages()
            {
                GeneratedMessages = new List<GeneratedMessage> { jobMessage }
            };
            var completedMessages = new List<Guid> { jobMessage.MessageId };
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetCompletedMessageIdentifiers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(completedMessages);
            var service = mocker.Create<JobMessageService>();
            await service.RecordStartedJobMessages(message, CancellationToken.None);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreInProgressMessageIdentifiers(It.Is<List<Guid>>(lst => !lst.Any()), It.IsAny<CancellationToken>()), Times.Once);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreCompletedMessageIdentifiers(It.Is<List<Guid>>(lst => !lst.Any()), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Ignores_Duplicate_Completed_Messages()
        {
            var message = new RecordJobMessageProcessingStatus
            {
                Id = Guid.NewGuid(),
                JobId = 1,
                GeneratedMessages = new List<GeneratedMessage>()
            };
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetCompletedMessageIdentifiersHistory(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid>{message.Id});
            var service = mocker.Create<JobMessageService>();
            await service.RecordCompletedJobMessageStatus(message);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreJobStatus(It.IsAny<JobStepStatus>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()), Times.Never);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreCompletedMessageIdentifiers(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

    }
}