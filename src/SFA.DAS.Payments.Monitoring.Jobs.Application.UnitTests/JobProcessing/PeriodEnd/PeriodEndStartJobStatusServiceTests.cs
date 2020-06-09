using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
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

        private List<(long? DcJobId, JobStatus JobStatus, bool? DcJobSucceeded, DateTimeOffset? endTime)>
            outstandingOrTimedOutJobs;

        [SetUp]
        public void SetUp()
        {
            inProgressMessages = new List<InProgressMessage>();
            completedMessages = new List<CompletedMessage>();
            outstandingOrTimedOutJobs = new List<(long? DcJobId, JobStatus JobStatus, bool? DcJobSucceeded, DateTimeOffset? endTime)>();
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

            outstandingOrTimedOutJobs.Add(CreateTimedOutSubmissionJob());

            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((hasFailedMessages: false, endTime: DateTimeOffset.UtcNow.AddSeconds(-10)));

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);

            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.CompletedWithErrors),
                        It.Is<DateTimeOffset>(endTime => endTime == completedMessage.CompletedTime),
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
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);

            outstandingOrTimedOutJobs[0] = CreateTimedOutSubmissionJob();

            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);

            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.CompletedWithErrors),
                        It.Is<DateTimeOffset>(endTime => endTime == completedMessage.CompletedTime),
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
            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false); //should not complete on first pass

            outstandingOrTimedOutJobs.Clear();
            outstandingOrTimedOutJobs.Add(CreateCompletedSubmissionJob());

            await service.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);

            mocker.Mock<IJobStorageService>()
                .Verify(
                    x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.Completed),
                        It.Is<DateTimeOffset>(endTime => endTime == completedMessage.CompletedTime),
                        It.IsAny<CancellationToken>()), Times.Once());
            
        }

        private (long? DcJobId, JobStatus JobStatus, bool? DcJobSucceeded, DateTimeOffset? endTime) CreateOutstandingSubmissionJob()
        {
            return (job.Id, JobStatus.InProgress, null, null);
        }

        private (long? DcJobId, JobStatus JobStatus, bool? DcJobSucceeded, DateTimeOffset? endTime) CreateTimedOutSubmissionJob()
        {
            return (job.Id, JobStatus.TimedOut, true, DateTimeOffset.UtcNow.AddSeconds(10));
        }

        private (long? DcJobId, JobStatus JobStatus, bool? DcJobSucceeded, DateTimeOffset? endTime)
            CreateCompletedSubmissionJob()
        {
            (long? DcJobId, JobStatus JobStatus, bool? DcJobSucceeded, DateTimeOffset? endTime) completedSubmissionJob =
                (job.Id, JobStatus.Completed, true, null);
            return completedSubmissionJob;
        }

        private static InProgressMessage CreateInProgressMessage(CompletedMessage completedMessage)
        {
            var inProgressMessage = new InProgressMessage
                {MessageId = completedMessage.MessageId, MessageName = "Message"};
            return inProgressMessage;
        }

        private CompletedMessage CreateCompletedMessage()
        {
            var completedMessage = new CompletedMessage
            {
                MessageId = Guid.NewGuid(),
                JobId = job.Id,
                Succeeded = true,
                CompletedTime = DateTimeOffset.UtcNow
            };
            return completedMessage;
        }
    }
}