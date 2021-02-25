using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class PeriodEndJobStatusServiceTests
    {
        private AutoMock mocker;
        private JobModel job;
        private List<CompletedMessage> completedMessages;
        private List<InProgressMessage> inProgressMessages;

        [SetUp]
        public void SetUp()
        {
            inProgressMessages = new List<InProgressMessage>();
            completedMessages = new List<CompletedMessage>();

            mocker = AutoMock.GetLoose();

            job = new JobModel
            {
                Id = 1,
                DcJobId = 99,
                StartTime = DateTimeOffset.UtcNow.AddSeconds(-30),
                Status = JobStatus.InProgress,
                LearnerCount = null,
                JobType = JobType.EarningsJob,
                DcJobEndTime = null,
                DcJobSucceeded = null,
                IlrSubmissionTime = null,
                Ukprn = null,
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
                .Returns(TimeSpan.FromSeconds(60));
            mocker.Mock<IJobServiceConfiguration>()
                .SetupGet(cfg => cfg.PeriodEndRunJobTimeout)
                .Returns(TimeSpan.FromSeconds(60));
        }

        [TestCase(JobType.PeriodEndRunJob)]
        [TestCase(JobType.PeriodEndStopJob)]
        public async Task Publishes_PeriodEndJobFinished_with_SUCCESS_When_PeriodEndJob_Finishes(JobType jobType)
        {
            job.JobType = jobType;
            job.DcJobSucceeded = null;

            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId, MessageName = "Message" };
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<PeriodEndJobStatusService>();
            await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    , It.Is<bool>(b => b == true)), Times.Once);
            mocker.Mock<IJobStatusEventPublisher>()

                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    , It.Is<bool>(b => b == false)), Times.Never);
        }

        [TestCase(JobType.PeriodEndStartJob)]
        [TestCase(JobType.PeriodEndStopJob)]
        public async Task Publishes_PeriodEndJobFinished_with_FAILURE_When_PeriodEndJob_TimesOut(JobType jobType)
        {
            job.JobType = jobType;
            job.LearnerCount = 0;
            job.StartTime = DateTimeOffset.UtcNow.AddSeconds(-2);
            job.DcJobSucceeded = null;
            mocker.Mock<IJobServiceConfiguration>()
                .SetupGet(cfg => cfg.EarningsJobTimeout)
                .Returns(TimeSpan.FromSeconds(1));

            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<PeriodEndJobStatusService>();
            await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStatusEventPublisher>()

                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    , It.Is<bool>(b => b == false)), Times.Once);

            mocker.Mock<IJobStatusEventPublisher>()

                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    , It.Is<bool>(b => b == true)), Times.Never);
        }

        [Test]
        public async Task Publishes_PeriodEndJobFinished_with_FAILURE_When_PeriodEndRunJob_TimesOut()
        {
            job.JobType = JobType.PeriodEndRunJob;
            job.LearnerCount = 0;
            job.StartTime = DateTimeOffset.UtcNow.AddSeconds(-2);
            job.DcJobSucceeded = null;
            mocker.Mock<IJobServiceConfiguration>()
                .SetupGet(cfg => cfg.PeriodEndRunJobTimeout)
                .Returns(TimeSpan.FromSeconds(1));

            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<PeriodEndJobStatusService>();
            await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStatusEventPublisher>()

                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    , It.Is<bool>(b => b == false)), Times.Once);

            mocker.Mock<IJobStatusEventPublisher>()

                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    , It.Is<bool>(b => b == true)), Times.Never);
        }

        [Test]
        public async Task Publishes_PeriodEndJobFinished_with_SUCCESS_When_No_OutstandingJobs_found()
        {
            job.JobType = JobType.PeriodEndStartJob;
            job.DcJobSucceeded = null;

            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId, MessageName = "Message" };
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(It.IsAny<long?>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OutstandingJobResult>());

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    , It.Is<bool>(b => b == true)), Times.Once);
            mocker.Mock<IJobStatusEventPublisher>()

                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    , It.Is<bool>(b => b == false)), Times.Never);
        }

        [Test]
        public async Task Publishes_PeriodEndJobFinished_with_SUCCESS_When_All_OutstandingJobs_are_Completed()
        {
            job.JobType = JobType.PeriodEndStartJob;
            job.DcJobSucceeded = null;

            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId, MessageName = "Message" };
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(It.IsAny<long?>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OutstandingJobResult>
                {
                    new OutstandingJobResult{ DcJobId = 1, DcJobSucceeded = true, JobStatus = JobStatus.Completed }
                });

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);
            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    , It.Is<bool>(b => b == true)), Times.Once);
            mocker.Mock<IJobStatusEventPublisher>()

                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    , It.Is<bool>(b => b == false)), Times.Never);
        }

        [Test]
        public async Task Publishes_PeriodEndJobFinished_with_FAILURE_When_TimedOut_OutstandingJobs_found()
        {
            job.JobType = JobType.PeriodEndStartJob;
            job.DcJobSucceeded = null;

            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId, MessageName = "Message" };
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(It.IsAny<long?>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OutstandingJobResult>
                {
                    new OutstandingJobResult{ DcJobId = 1, DcJobSucceeded = true, JobStatus = JobStatus.TimedOut, EndTime = DateTimeOffset.Now.AddMinutes(1) }
                });

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);

            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job),
                    It.Is<bool>(b => b == true)), Times.Never);

            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job),
                    It.Is<bool>(b => b == false)), Times.Once);
        }

        [Test]
        public async Task Returns_False_And_Does_Not_Publishes_PeriodEndJobFinished_When_DAS_InProgress_DcSucceeded_OutstandingJobs_found()
        {
            job.JobType = JobType.PeriodEndStartJob;
            job.DcJobSucceeded = null;

            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId, MessageName = "Message" };
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(It.IsAny<long?>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OutstandingJobResult>
                {
                    new OutstandingJobResult{ DcJobId = 1, DcJobSucceeded = true, JobStatus = JobStatus.InProgress }
                });

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);

            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job),
                    It.IsAny<bool>()), Times.Never);
        
            result.Should().BeFalse();
        }

        [Test]
        public async Task Returns_False_And_Does_Not_Publishes_PeriodEndJobFinished_When_DAS_InProgress_DcFailed_OutstandingJobs_found()
        {
            job.JobType = JobType.PeriodEndStartJob;
            job.DcJobSucceeded = null;

            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId, MessageName = "Message" };
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(It.IsAny<long?>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OutstandingJobResult>
                {
                    new OutstandingJobResult{ DcJobId = 1, DcJobSucceeded = false, JobStatus = JobStatus.InProgress }
                });

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);

            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job),
                    It.IsAny<bool>()), Times.Never);

            result.Should().BeFalse();
        }

        [Test]
        public async Task Returns_False_And_Does_Not_Publishes_PeriodEndJobFinished_When_DAS_Completed_DCStatus_Null_OutstandingJobs_found()
        {
            job.JobType = JobType.PeriodEndStartJob;
            job.DcJobSucceeded = null;

            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId, MessageName = "Message" };
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(It.IsAny<long?>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OutstandingJobResult>
                {
                    new OutstandingJobResult{ DcJobId = 1, DcJobSucceeded = null, JobStatus = JobStatus.Completed }
                });

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);

            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job),
                    It.IsAny<bool>()), Times.Never);

            result.Should().BeFalse();
        }

        [Test]
        public async Task Returns_False_And_Does_Not_Publishes_PeriodEndJobFinished_When_DAS_InProgress_DCStatus_Null_OutstandingJobs_found()
        {
            job.JobType = JobType.PeriodEndStartJob;
            job.DcJobSucceeded = null;

            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            var inProgressMessage = new InProgressMessage { MessageId = completedMessage.MessageId, MessageName = "Message" };
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(It.IsAny<long?>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OutstandingJobResult>
                {
                    new OutstandingJobResult{ DcJobId = 1, DcJobSucceeded = null, JobStatus = JobStatus.InProgress }
                });

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.ManageStatus(job.Id, CancellationToken.None).ConfigureAwait(false);

            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job),
                    It.IsAny<bool>()), Times.Never);

            result.Should().BeFalse();
        }
    }
}