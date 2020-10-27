using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.Earnings;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class EarningsJobStatusServiceTests
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
                {JobStepStatus.Completed, 10}
            };
            mocker = AutoMock.GetLoose();
            lastJobStepEndTime = DateTimeOffset.UtcNow;

            job = new JobModel
            {
                Id = 1,
                DcJobId = 99,
                StartTime = DateTimeOffset.UtcNow.AddSeconds(-10),
                Status = JobStatus.InProgress,
                LearnerCount = 100,
                JobType = JobType.EarningsJob,
                DcJobEndTime = DateTimeOffset.Now,
                DcJobSucceeded = true,
                IlrSubmissionTime = DateTime.Now,
                Ukprn = 1234,
                AcademicYear = 1920,
                CollectionPeriod = 01
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
            mocker.Mock<IJobServiceConfiguration>()
                .SetupGet(cfg => cfg.EarningsJobTimeout)
                .Returns(TimeSpan.FromSeconds(20));
        }

        [Test]
        public async Task Removes_Completed_Job_Messages_From_Cache()
        {
            var jobId = 99;
            var completedMessageId = Guid.NewGuid();
            var inProgress = new InProgressMessage {MessageId = completedMessageId};
            inProgressMessages.Add(inProgress);
            completedMessages.Add(new CompletedMessage
            {
                MessageId = completedMessageId, JobId = jobId, Succeeded = true, CompletedTime = DateTimeOffset.UtcNow
            });
            var service = mocker.Create<EarningsJobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(svc => svc.RemoveInProgressMessages(It.Is<long>(id => id == jobId),
                    It.Is<List<Guid>>(identifiers =>
                        identifiers.Count == 1 && identifiers.Contains(completedMessageId)),
                    It.IsAny<CancellationToken>()), Times.Once);
            mocker.Mock<IJobStorageService>()
                .Verify(svc => svc.RemoveCompletedMessages(It.Is<long>(id => id == jobId),
                    It.Is<List<Guid>>(identifiers =>
                        identifiers.Count == 1 && identifiers.Contains(completedMessageId)),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Does_Not_Complete_Job_If_Has_InProgress_Messages_Waiting_For_Completion()
        {
            var jobId = 99;
            var completedMessageId = Guid.NewGuid();
            inProgressMessages.Add(new InProgressMessage {MessageId = Guid.NewGuid()});
            inProgressMessages.Add(new InProgressMessage {MessageId = Guid.NewGuid()});
            var service = mocker.Create<EarningsJobStatusService>();
            var status = await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            status.Should().BeFalse();
        }

        [Test]
        public async Task Stores_Latest_Message_Completion_Time()
        {
            var jobId = 99;
            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage {MessageId = completedMessage.MessageId};
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<EarningsJobStatusService>();
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
            var inProgressMessage = new InProgressMessage
                {MessageId = completedMessage.MessageId, MessageName = "Message"};
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<EarningsJobStatusService>();
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
            var inProgressMessage = new InProgressMessage
                {MessageId = completedMessage.MessageId, MessageName = "Message"};
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<EarningsJobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.Completed),
                        It.Is<DateTimeOffset>(endTime => endTime == completedMessage.CompletedTime),
                        It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task Does_Not_Return_Finished_If_All_Messages_Completed_But_DC_Not_Confirmed_Completed()
        {
            var jobId = 99;
            job.DcJobEndTime = null;
            job.DcJobSucceeded = null;
            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage
                {MessageId = completedMessage.MessageId, MessageName = "Message"};
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<EarningsJobStatusService>();
            var finished = await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            finished.Should().BeFalse();
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
            var inProgressMessage = new InProgressMessage
                {MessageId = completedMessage.MessageId, MessageName = "Message"};
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: true, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<EarningsJobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.CompletedWithErrors),
                        It.Is<DateTimeOffset>(endTime => endTime == completedMessage.CompletedTime),
                        It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestCase("PayableEarningEvent")]
        [TestCase("EarningFailedDataLockMatching")]
        [TestCase("FunctionalSkillEarningFailedDataLockMatching")]
        [TestCase("PayableFunctionalSkillEarningEvent")]
        [TestCase("ProcessLearnerCommand")]
        [TestCase("Act1FunctionalSkillEarningsEvent")]
        [TestCase("ApprenticeshipContractType1EarningEvent")]
        public async Task Records_DataLocks_Completion_Time_After_Processing_DataLocks_Events(string messageName)
        {
            var jobId = 99;
            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow,
            };

            var inProgressMessage = new InProgressMessage
                {MessageName = messageName, MessageId = completedMessage.MessageId};
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<EarningsJobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveDataLocksCompletionTime(It.Is<long>(id => id == jobId),
                        It.Is<DateTimeOffset>(endTime => endTime == completedMessage.CompletedTime),
                        It.IsAny<CancellationToken>()), Times.Once());

        }

        [Test]
        public async Task Records_Earnings_Job_As_Completed_If_No_Learners()
        {
            var jobId = 99;
            job.LearnerCount = 0;
            inProgressMessages.Clear();
            completedMessages.Clear();

            var service = mocker.Create<EarningsJobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.Completed),
                        It.Is<DateTimeOffset>(endTime => endTime == job.StartTime),
                        It.IsAny<CancellationToken>()), Times.Once());
        }


        //[Test]
        //public async Task Does_Not_record_Job_As_Completed_If_No_Learners_And_Not_Earnings_Job()
        //{
        //    var jobId = 99;
        //    job.LearnerCount = 0;
        //    job.JobType = JobType.PeriodEndRunJob;
        //    var completedMessage = new CompletedMessage
        //    {
        //        MessageId = Guid.NewGuid(),
        //        JobId = job.Id,
        //        Succeeded = true,
        //        CompletedTime = DateTimeOffset.UtcNow
        //    };
        //    var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId, MessageName = "Message" };
        //    inProgressMessages.Add(inProgressMessage);
        //    inProgressMessages.Add(new InProgressMessage { MessageId = Guid.NewGuid(), MessageName = "Message" });
        //    completedMessages.Add(completedMessage);

        //    var service = mocker.Create<EarningsJobStatusService>();
        //    await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
        //    mocker.Mock<IJobStorageService>()
        //        .Verify(
        //            x => x.SaveJobStatus(It.IsAny<long>(),
        //                It.IsAny<JobStatus>(),
        //                It.IsAny<DateTimeOffset>(),
        //                It.IsAny<CancellationToken>()), Times.Never);
        //}

        [Test]
        public async Task Publishes_JobFinished_When_Job_Finished_And_Recorded_DC_Completion()
        {
            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage
                {MessageId = completedMessage.MessageId, MessageName = "Message"};
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<EarningsJobStatusService>();
            await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(publisher => publisher.SubmissionFinished(It.Is<bool>(succeeded => succeeded),
                    It.Is<long>(id => id == job.DcJobId),
                    It.Is<long>(ukprn => ukprn == job.Ukprn),
                    It.Is<short>(academicYEar => academicYEar == job.AcademicYear),
                    It.Is<byte>(collectionPeriod => collectionPeriod == job.CollectionPeriod),
                    It.Is<DateTime>(ilrSubmissionTIme => ilrSubmissionTIme == job.IlrSubmissionTime)), Times.Once);
        }

        [Test]
        public async Task Publishes_JobFinished_When_Job_Finished_And_Recorded_DC_Failure()
        {
            job.DcJobSucceeded = false;
            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage
                {MessageId = completedMessage.MessageId, MessageName = "Message"};
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<EarningsJobStatusService>();
            await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(publisher => publisher.SubmissionFinished(It.Is<bool>(succeeded => !succeeded),
                    It.Is<long>(id => id == job.DcJobId),
                    It.Is<long>(ukprn => ukprn == job.Ukprn),
                    It.Is<short>(academicYEar => academicYEar == job.AcademicYear),
                    It.Is<byte>(collectionPeriod => collectionPeriod == job.CollectionPeriod),
                    It.Is<DateTime>(ilrSubmissionTIme => ilrSubmissionTIme == job.IlrSubmissionTime)), Times.Once);
        }

        [Test]
        public async Task Records_Job_As_Failed_If_Dc_Tasks_Fails()
        {
            var jobId = 99;
            job.LearnerCount = 0;
            job.StartTime = DateTimeOffset.UtcNow.AddSeconds(-2);
            job.DcJobSucceeded = false;
            mocker.Mock<IJobServiceConfiguration>()
                .SetupGet(cfg => cfg.EarningsJobTimeout)
                .Returns(TimeSpan.FromSeconds(1));

            var service = mocker.Create<EarningsJobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.DcTasksFailed),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<CancellationToken>()),
                    Times.Once());
        }

        [Test]
        public async Task Records_Job_As_Failed_If_Dc_Tasks_Fails_But_Payments_Completed()
        {
            var jobId = 99;
            job.LearnerCount = 0;
            job.StartTime = DateTimeOffset.UtcNow.AddSeconds(-2);
            job.DcJobSucceeded = false;
            job.Status = JobStatus.Completed;
            mocker.Mock<IJobServiceConfiguration>()
                .SetupGet(cfg => cfg.EarningsJobTimeout)
                .Returns(TimeSpan.FromSeconds(1));

            var service = mocker.Create<EarningsJobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.DcTasksFailed),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<CancellationToken>()),
                    Times.Once());
        }

        [Test]
        public async Task Records_Job_As_Completed_With_Errors_If_Dc_Tasks_Succeeds_But_Times_Out()
        {
            var jobId = 99;
            job.LearnerCount = 0;
            job.StartTime = DateTimeOffset.UtcNow.AddSeconds(-2);
            job.DcJobSucceeded = true;
            mocker.Mock<IJobServiceConfiguration>()
                .SetupGet(cfg => cfg.EarningsJobTimeout)
                .Returns(TimeSpan.FromSeconds(1));

            var service = mocker.Create<EarningsJobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.CompletedWithErrors),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<CancellationToken>()),
                    Times.Once());
        }

        [Test]
        public async Task Records_Job_As_Timed_Out_If_Dc_Tasks_Not_Completed()
        {
            var jobId = 99;
            job.LearnerCount = 0;
            job.StartTime = DateTimeOffset.UtcNow.AddSeconds(-2);
            job.DcJobSucceeded = null;
            mocker.Mock<IJobServiceConfiguration>()
                .SetupGet(cfg => cfg.EarningsJobTimeout)
                .Returns(TimeSpan.FromSeconds(1));

            var service = mocker.Create<EarningsJobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.TimedOut),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<CancellationToken>()),
                    Times.Once());
        }

    }
}