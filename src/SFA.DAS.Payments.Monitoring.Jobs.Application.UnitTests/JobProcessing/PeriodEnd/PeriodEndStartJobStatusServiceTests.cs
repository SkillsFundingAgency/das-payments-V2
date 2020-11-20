using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests.JobProcessing.PeriodEnd
{
    [TestFixture]
    public class PeriodEndStartJobStatusServiceTests
    {
        private AutoMock mocker;

        private JobModel job;
        private List<CompletedMessage> completedMessages;
        private List<InProgressMessage> inProgressMessages;

        private List<OutstandingJobResult>
            outstandingOrTimedOutJobs;

        [SetUp]
        public void SetUp()
        {
            inProgressMessages = new List<InProgressMessage>();
            completedMessages = new List<CompletedMessage>();
            outstandingOrTimedOutJobs = new List<OutstandingJobResult>();
            mocker = AutoMock.GetLoose();

            job = new JobModel
            {
                Id = 1,
                DcJobId = 99,
                StartTime = DateTimeOffset.UtcNow.AddSeconds(-1),
                Status = JobStatus.InProgress,
                LearnerCount = null,
                JobType = JobType.PeriodEndStartJob,
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

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(It.IsAny<long>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(outstandingOrTimedOutJobs);

        }

        [Test]
        public async Task ManageStatus_GiveAtLeastOneTimedOutJobs_FailsWithCompletedWithErrorsStatus()
        {
            var jobId = 99;
            var completedMessage = CreateCompletedMessage();
            var inProgressMessage = CreateInProgressMessage(completedMessage);
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);

            var timedOutSubmissionJob = CreateTimedOutSubmissionJob();
            outstandingOrTimedOutJobs.Add(timedOutSubmissionJob);

            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            result.Should().BeTrue();
            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.CompletedWithErrors),
                        It.Is<DateTimeOffset>(endTime => endTime == timedOutSubmissionJob.EndTime),
                        It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task ManageStatus_GivenMultipleSubmissionJobsWhereOneTimesOut_FailsFastOnFirstFailure()
        {
            var jobId = 99;
            var completedMessage = CreateCompletedMessage();
            var inProgressMessage = CreateInProgressMessage(completedMessage);
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);

            outstandingOrTimedOutJobs.Add(CreateOutstandingSubmissionJob());
            outstandingOrTimedOutJobs.Add(CreateOutstandingSubmissionJob());
            outstandingOrTimedOutJobs.Add(CreateOutstandingSubmissionJob());

            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result = await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            result.Should().BeFalse();

            outstandingOrTimedOutJobs[0] = CreateTimedOutSubmissionJob();

            result = await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            result.Should().BeTrue();

            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.CompletedWithErrors),
                        It.Is<DateTimeOffset>(endTime => endTime == outstandingOrTimedOutJobs[0].EndTime),
                        It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task ManageStatus_ContinuesUntilAllInProgressJobsHaveCompleted()
        {
            var jobId = 99;
            var completedMessage = CreateCompletedMessage();
            var inProgressMessage = CreateInProgressMessage(completedMessage);
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);

            outstandingOrTimedOutJobs.Add(CreateOutstandingSubmissionJob());

            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result =
                await service.ManageStatus(jobId, CancellationToken.None)
                    .ConfigureAwait(false); //should not complete on first pass
            result.Should().BeFalse();
            CompleteJob(outstandingOrTimedOutJobs[0]);

            result = await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            result.Should().BeTrue();

            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.Completed),
                        It.Is<DateTimeOffset>(endTime => endTime == outstandingOrTimedOutJobs[0].EndTime),
                        It.IsAny<CancellationToken>()), Times.Once());

        }

        [Test]
        public async Task ManageStatus_ContinuesUntilAllInProgressAndUsesLastEndTimeAsJobEndTime()
        {
            var jobId = 99;
            var completedMessage = CreateCompletedMessage();
            var inProgressMessage = CreateInProgressMessage(completedMessage);
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);

            outstandingOrTimedOutJobs.Add(CreateOutstandingSubmissionJob());
            outstandingOrTimedOutJobs.Add(CreateOutstandingSubmissionJob());

            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            var result =
                await service.ManageStatus(jobId, CancellationToken.None)
                    .ConfigureAwait(false); //should not complete on first pass
            result.Should().BeFalse();

            CompleteJob(outstandingOrTimedOutJobs[0]);
            CompleteJob(outstandingOrTimedOutJobs[1]);

            result = await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            result.Should().BeTrue();

            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.Completed),
                        It.Is<DateTimeOffset>(endTime => endTime == outstandingOrTimedOutJobs[1].EndTime),
                        It.IsAny<CancellationToken>()), Times.Once());

        }

        [Test]
        public async Task ManageStatus_EndTimeShouldUseLastActivityCompleted()
        {
            var jobId = 99;
            var completedMessage = CreateCompletedMessage();
            completedMessage.CompletedTime = DateTimeOffset.Now.AddMinutes(1); //will be the last completed job

            var inProgressMessage = CreateInProgressMessage(completedMessage);
            inProgressMessages.Add(inProgressMessage);
            completedMessages.Add(completedMessage);

            outstandingOrTimedOutJobs.Add(CreateCompletedSubmissionJob());//well complete prior to inprogress/completed messages

            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<PeriodEndStartJobStatusService>();

            var result = await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            result.Should().BeTrue();

            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.Completed),
                        It.Is<DateTimeOffset>(endTime => endTime == completedMessage.CompletedTime),
                        It.IsAny<CancellationToken>()), Times.Once());
        }

        private void CompleteJob(OutstandingJobResult outstandingJob)
        {
            outstandingJob.JobStatus = JobStatus.Completed;
            outstandingJob.EndTime = DateTimeOffset.UtcNow;
            outstandingJob.DcJobSucceeded = true;
        }

        private OutstandingJobResult CreateOutstandingSubmissionJob()
        {
            return new OutstandingJobResult { DcJobId = job.Id, JobStatus = JobStatus.InProgress, DcJobSucceeded = null, EndTime = null };
        }

        private OutstandingJobResult CreateTimedOutSubmissionJob()
        {
            return new OutstandingJobResult { DcJobId = job.Id, JobStatus = JobStatus.TimedOut, DcJobSucceeded = true, EndTime = DateTimeOffset.UtcNow.AddSeconds(10) };
        }

        private OutstandingJobResult CreateCompletedSubmissionJob()
        {
            return new OutstandingJobResult { DcJobId = job.Id, JobStatus = JobStatus.Completed, DcJobSucceeded = true, EndTime = DateTimeOffset.UtcNow.AddSeconds(10) };
        }

        private static InProgressMessage CreateInProgressMessage(CompletedMessage completedMessage)
        {
            return new InProgressMessage { MessageId = completedMessage.MessageId, MessageName = "Message" };
        }

        private CompletedMessage CreateCompletedMessage()
        {
            return new CompletedMessage { MessageId = Guid.NewGuid(), JobId = job.Id, Succeeded = true, CompletedTime = DateTimeOffset.UtcNow };
        }
    }
}