using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
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
        private List<Guid> completedMessages;
        private List<Guid> inProgressMessages;

        [SetUp]
        public void SetUp()
        {
            inProgressMessages = new List<Guid>();
            completedMessages = new List<Guid>();
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
                Status = JobStatus.Completed
            };
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJob(It.IsAny<CancellationToken>()))
                .ReturnsAsync(job);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetInProgressMessageIdentifiers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid>());
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetCompletedMessageIdentifiers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid>());
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobStepStatus.Completed, DateTime.UtcNow));
        }

        [Test]
        public async Task Completes_Job_If_No_Job_Messages_Stored()
        {
            var service = mocker.Create<JobStatusService>();
            var status = await service.ManageStatus().ConfigureAwait(false);
            status.Should().Be(JobStatus.Completed);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.UpdateJob(It.Is<JobModel>(model => model.Status == JobStatus.Completed), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Completes_Job_As_Completed_With_Errors_If_Any_Messages_Failed()
        {
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobStepStatus.Failed, DateTime.UtcNow));
            var service = mocker.Create<JobStatusService>();
            var status = await service.ManageStatus().ConfigureAwait(false);
            status.Should().Be(JobStatus.CompletedWithErrors);
        }
        
        [Test]
        public async Task Does_Not_Complete_Job_If_Not_All_Messages_Finished()
        {
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetInProgressMessageIdentifiers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid>{Guid.NewGuid()});
            var service = mocker.Create<JobStatusService>();
            var status = await service.ManageStatus().ConfigureAwait(false);
            status.Should().Be(JobStatus.InProgress);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.UpdateJob(It.IsAny<JobModel>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Does_Not_Complete_Job_If_Waiting_For_Job_Message_Starts()
        {
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetCompletedMessageIdentifiers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid> { Guid.NewGuid() });
            var service = mocker.Create<JobStatusService>();
            var status = await service.ManageStatus().ConfigureAwait(false);
            status.Should().Be(JobStatus.InProgress);
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.UpdateJob(It.IsAny<JobModel>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}