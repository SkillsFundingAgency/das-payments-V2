using System;
using System.Collections.Generic;
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
                .Setup(x => x.GetInProgressMessageIdentifiers( It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid>());
        }

        [Test]
        public async Task Stores_Completed_Job_Message()
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
            await service.RecordCompletedJobMessageStatus(jobStatusMessage, default(CancellationToken));
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.StoreCompletedMessage(It.Is<CompletedMessage>(msg =>
                        msg.MessageId == jobStatusMessage.Id && msg.CompletedTime == jobStatusMessage.EndTime),It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Test]
        public async Task Stores_In_Progress_Messages()
        {
            var generatedMessageA = new GeneratedMessage
            {

                StartTime = DateTimeOffset.UtcNow,
                MessageId = Guid.NewGuid(),
                MessageName = "MessageA",
            };
            var generatedMessageB = new GeneratedMessage
            {

                StartTime = DateTimeOffset.UtcNow,
                MessageId = Guid.NewGuid(),
                MessageName = "MessageB",
            };
            var jobStatusMessage = new RecordJobMessageProcessingStatus
            {
                Id = Guid.NewGuid(),
                JobId = 1,
                GeneratedMessages = new List<GeneratedMessage>{ generatedMessageA, generatedMessageB },
                EndTime = DateTime.UtcNow,
                Succeeded = true
            };

            var service = mocker.Create<JobMessageService>();
            await service.RecordCompletedJobMessageStatus(jobStatusMessage, CancellationToken.None);

            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreInProgressMessageIdentifiers(It.Is<long>(jobId => jobId == jobStatusMessage.JobId), It.Is<List<Guid>>(identifiers =>
                    identifiers.Count == 2 &&
                    identifiers.Contains(generatedMessageA.MessageId) &&
                    identifiers.Contains(generatedMessageB.MessageId)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}