using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
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
        private List<CompletedMessage> completedMessages;
        private List<InProgressMessage> inProgressMessages;

        [SetUp]
        public void SetUp()
        {
            inProgressMessages = new List<InProgressMessage>();
            completedMessages = new List<CompletedMessage>();
            stepsStatuses = new Dictionary<JobStepStatus, int>()
            {
                {JobStepStatus.Completed, 10 }
            };
            mocker = AutoMock.GetLoose();
            lastJobStepEndTime = DateTimeOffset.UtcNow;

            job = new JobModel
            {
                Id = 1,
                DcJobId = 99,
                StartTime = DateTimeOffset.UtcNow.AddSeconds(-10),
                Status = JobStatus.InProgress
            };
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJob(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(job);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetInProgressMessages(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inProgressMessages);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetCompletedMessages(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(completedMessages);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: null));
        }

        [Test]
        public async Task Removes_Completed_Job_Messages_From_Cache()
        {
            var jobId = 99;
            var completedMessageId = Guid.NewGuid();
            var inProgress = new InProgressMessage{MessageId = completedMessageId};
            inProgressMessages.Add(inProgress);
            completedMessages.Add(new CompletedMessage { MessageId = completedMessageId, JobId = jobId, Succeeded = true, CompletedTime = DateTimeOffset.UtcNow });
            var service = mocker.Create<JobStatusService>();
            var status = await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            status.Should().Be(JobStatus.Completed);
            mocker.Mock<IJobStorageService>()
                .Verify(svc => svc.RemoveInProgressMessages(It.Is<long>(id => id == jobId),
                    It.Is<List<Guid>>(identifiers => identifiers.Count == 1 && identifiers.Contains(completedMessageId)), It.IsAny<CancellationToken>()), Times.Once);
            mocker.Mock<IJobStorageService>()
                .Verify(svc => svc.RemoveCompletedMessages(It.Is<long>(id => id == jobId),
                    It.Is<List<Guid>>(identifiers => identifiers.Count == 1 && identifiers.Contains(completedMessageId)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Does_Not_Complete_Job_If_Has_InProgress_Messages_Waiting_For_Completion()
        {
            var jobId = 99;
            var completedMessageId = Guid.NewGuid();
            inProgressMessages.Add(new InProgressMessage{MessageId = Guid.NewGuid()});
            inProgressMessages.Add(new InProgressMessage { MessageId = Guid.NewGuid() });
            var service = mocker.Create<JobStatusService>();
            var status = await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            status.Should().Be(JobStatus.InProgress);
        }

        [Test]
        public async Task Stores_Latest_Message_Completion_Time()
        {
            var jobId = 99;
            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(), JobId = job.Id, Succeeded = true, CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage {MessageId =completedMessage.MessageId};
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<JobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.StoreJobStatus(It.Is<long>(id => id == jobId), It.IsAny<bool>(),
                        It.Is<DateTimeOffset?>(endTime => endTime.Value == completedMessage.CompletedTime),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Records_Failed_Message_Status()
        {
            var jobId = 99;
            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = false,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId };
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<JobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.StoreJobStatus(It.Is<long>(id => id == jobId), It.Is<bool>(failed => failed),
                        It.IsAny<DateTimeOffset?>(),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Records_Job_Completion_Status_If_All_Messages_Completed()
        {
            var jobId = 99;
            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId };
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<JobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId), 
                        It.Is<JobStatus>(status => status == JobStatus.Completed),
                        It.Is<DateTimeOffset>(endTime => endTime == completedMessage.CompletedTime),
                        It.IsAny<CancellationToken>()), Times.Once());
        }


        [Test]
        public async Task Records_Job_Completed_With_Errors()
        {
            var jobId = 99;
            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId };
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: true, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<JobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.CompletedWithErrors),
                        It.Is<DateTimeOffset>(endTime => endTime == completedMessage.CompletedTime),
                        It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}