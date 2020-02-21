using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using FluentAssertions;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.UnitTests
{
    [TestFixture]
    public class JobStorageServiceTests
    {
        private JobStorageService jobsStorageService;
        private Mock<IJobModelRepository> jobModelRepository;
        private Mock<IInProgressMessageRepository> inProgressMessageRepository;
        private Mock<ICompletedMessageRepository> completedMessageRepository;
        private Mock<IJobStatusRepository> jobStatusRepository;

        private long dcJobId = 114;
        private DateTimeOffset updatedEndTime = DateTimeOffset.MaxValue;
        private JobStatus updatedJobStatus = JobStatus.TimedOut;

        private JobModel job;

        [SetUp]
        public void Setup()
        {
            using (var mocker = AutoMock.GetLoose())
            {
                jobModelRepository = mocker.Mock<IJobModelRepository>();
                inProgressMessageRepository = mocker.Mock<IInProgressMessageRepository>();
                completedMessageRepository = mocker.Mock<ICompletedMessageRepository>();
                jobStatusRepository = mocker.Mock<IJobStatusRepository>();
                jobsStorageService = new JobStorageService(
                    mocker.Mock<IPaymentLogger>().Object,
                    jobModelRepository.Object,
                    inProgressMessageRepository.Object,
                    completedMessageRepository.Object,
                    jobStatusRepository.Object);
            }

            job = new JobModel
            {
                DcJobId = dcJobId,
                Status = JobStatus.InProgress,
                DcJobEndTime = null
            };
        }

        [Test]
        public async Task StoreNewJob_Validates_DCJobId()
        {
            job.DcJobId = null;
            Func<Task> action = async () => { await jobsStorageService.StoreNewJob(job, CancellationToken.None); };
            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public async Task StoreNewJob_Checks_Existing_Job()
        {
            await jobsStorageService.StoreNewJob(job, CancellationToken.None);
            jobModelRepository.Verify(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreNewJob_Returns_False_If_Job_Is_Already_Stored()
        {
            jobModelRepository.Setup(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>())).ReturnsAsync(job);
            var actualResult = await jobsStorageService.StoreNewJob(job, CancellationToken.None);
            actualResult.Should().BeFalse();
        }

        [Test]
        public async Task StoreNewJob_Calls_UpsertJob()
        {
            await jobsStorageService.StoreNewJob(job, CancellationToken.None);
            jobModelRepository.Verify(x => x.UpsertJob(job, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreNewJob_Returns_True_When_Job_Is_Stored()
        {
            var actualResult = await jobsStorageService.StoreNewJob(job, CancellationToken.None);
            actualResult.Should().BeTrue();
        }

        [Test]
        public async Task SaveJobStatus_Calls_GetJob()
        {
            jobModelRepository.Setup(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>())).ReturnsAsync(job);
            await jobsStorageService.SaveJobStatus(dcJobId, updatedJobStatus, updatedEndTime, CancellationToken.None);
            jobModelRepository.Verify(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task SaveJobStatus_Throws_Exception_When_Job_Not_In_Cache()
        {
            Func<Task> action = async () => { await jobsStorageService.SaveJobStatus(dcJobId, updatedJobStatus, updatedEndTime, CancellationToken.None); };
            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public async Task SaveJobStatus_Calls_Upsert_Job_With_Updated_Status()
        {
            jobModelRepository.Setup(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>())).ReturnsAsync(job);
            await jobsStorageService.SaveJobStatus(dcJobId, updatedJobStatus, updatedEndTime, CancellationToken.None);
            jobModelRepository.Verify(x => x.UpsertJob(It.Is<JobModel>(y => y.DcJobId == dcJobId && y.Status == updatedJobStatus && y.EndTime == updatedEndTime), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetCurrentJobs_Queries_The_Repository_Correctly()
        {
            await jobsStorageService.GetCurrentJobs(CancellationToken.None);
            jobModelRepository.Verify(x => x.GetJobIdsByQuery(It.IsAny<Func<JobModel,bool>>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetCurrentJobs_Returns_The_Expected_Jobs()
        {
            await jobsStorageService.GetCurrentJobs(CancellationToken.None);
            jobModelRepository.Verify(x => x.GetJobIdsByQuery(It.IsAny<Func<JobModel, bool>>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreDcJobStatus_Calls_GetJob()
        {
            jobModelRepository.Setup(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>())).ReturnsAsync(job);
            await jobsStorageService.StoreDcJobStatus(dcJobId, true, CancellationToken.None);
            jobModelRepository.Verify(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreDcJobStatus_Throws_Exception_When_Job_Not_In_Cache()
        {
            Func<Task> action = async () => { await jobsStorageService.StoreDcJobStatus(dcJobId, true, CancellationToken.None); };
            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public async Task StoreDcJobStatus_Calls_Upsert_Job_With_Updated_Succeeded()
        {
            jobModelRepository.Setup(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>())).ReturnsAsync(job);
            await jobsStorageService.StoreDcJobStatus(dcJobId, true, CancellationToken.None);
            jobModelRepository.Verify(x => x.UpsertJob(It.Is<JobModel>(y => y.DcJobSucceeded == true), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreDcJobStatus_Calls_Upsert_Job_With_Updated_DcJobEndTime()
        {
            jobModelRepository.Setup(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>())).ReturnsAsync(job);
            await jobsStorageService.StoreDcJobStatus(dcJobId, true, CancellationToken.None);
            jobModelRepository.Verify(x => x.UpsertJob(It.Is<JobModel>(y => y.DcJobEndTime != null), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreDcJobStatus_Calls_Upsert_Job_With_Updated_End_Time_On_Timeout()
        {
            job.Status = JobStatus.TimedOut;
            jobModelRepository.Setup(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>())).ReturnsAsync(job);
            await jobsStorageService.StoreDcJobStatus(dcJobId, true, CancellationToken.None);
            jobModelRepository.Verify(x => x.UpsertJob(It.Is<JobModel>(y => y.EndTime != null), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreDcJobStatus_Calls_Upsert_Job_With_Updated_Status_On_Timeout_Success()
        {
            job.Status = JobStatus.TimedOut;
            jobModelRepository.Setup(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>())).ReturnsAsync(job);
            await jobsStorageService.StoreDcJobStatus(dcJobId, true, CancellationToken.None);
            jobModelRepository.Verify(x => x.UpsertJob(It.Is<JobModel>(y => y.Status == JobStatus.CompletedWithErrors), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreDcJobStatus_Calls_Upsert_Job_With_Updated_Status_On_Timeout_Failure()
        {
            job.Status = JobStatus.TimedOut;
            jobModelRepository.Setup(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>())).ReturnsAsync(job);
            await jobsStorageService.StoreDcJobStatus(dcJobId, false, CancellationToken.None);
            jobModelRepository.Verify(x => x.UpsertJob(It.Is<JobModel>(y => y.Status == JobStatus.DcTasksFailed), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetJob_Calls_GetJob_On_Repository()
        {
            await jobsStorageService.GetJob(dcJobId, CancellationToken.None);
            jobModelRepository.Verify(x => x.GetJob(dcJobId, CancellationToken.None));
        }

        [Test]
        public async Task GetJob_Returns_The_Job_Given_From_The_Repository()
        {
            var returnedJobModel = new JobModel{ AcademicYear = 1718, CollectionPeriod = 5 };
            jobModelRepository.Setup(x => x.GetJob(dcJobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnedJobModel);
            var actualResult = await jobsStorageService.GetJob(dcJobId, CancellationToken.None);
            actualResult.Should().Be(returnedJobModel);
        }

        [Test]
        public async Task GetInProgressMessages()
        {
            var expectedMessages = new List<InProgressMessage>
            {
                new InProgressMessage{ JobId = dcJobId, MessageId = Guid.NewGuid(), MessageName = "TestOne" },
                new InProgressMessage{ JobId = dcJobId, MessageId = Guid.NewGuid(), MessageName = "TestTwo" }
            };
            inProgressMessageRepository.Setup(x => x.GetOrAddInProgressMessages(dcJobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedMessages);

            var actualMessages = await jobsStorageService.GetInProgressMessages(dcJobId, CancellationToken.None);

            actualMessages.Should().BeSameAs(expectedMessages);
            inProgressMessageRepository.Verify(x => x.GetOrAddInProgressMessages(dcJobId, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task RemoveInProgressMessages()
        {
            var expectedMessageIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };
            
            await jobsStorageService.RemoveInProgressMessages(dcJobId, expectedMessageIds, CancellationToken.None);

            inProgressMessageRepository.Verify(x =>
                x.RemoveInProgressMessages(dcJobId, expectedMessageIds, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreInProgressMessages()
        {
            var expectedMessages = new List<InProgressMessage>
            {
                new InProgressMessage{ JobId = dcJobId, MessageId = Guid.NewGuid(), MessageName = "TestOne" },
                new InProgressMessage{ JobId = dcJobId, MessageId = Guid.NewGuid(), MessageName = "TestTwo" }
            };

            await jobsStorageService.StoreInProgressMessages(dcJobId, expectedMessages, CancellationToken.None);

            inProgressMessageRepository.Verify(x =>
                x.StoreInProgressMessages(dcJobId, expectedMessages, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetCompletedMessages()
        {
            var expectedMessages = new List<CompletedMessage>
            {
                new CompletedMessage{ JobId = dcJobId, MessageId = Guid.NewGuid(), Succeeded = true },
                new CompletedMessage{ JobId = dcJobId, MessageId = Guid.NewGuid(), Succeeded = false }
            };
            completedMessageRepository.Setup(x => x.GetOrAddCompletedMessages(dcJobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedMessages);

            var actualMessages = await jobsStorageService.GetCompletedMessages(dcJobId, CancellationToken.None);

            actualMessages.Should().BeSameAs(expectedMessages);
            completedMessageRepository.Verify(x => x.GetOrAddCompletedMessages(dcJobId, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task RemoveCompletedMessages()
        {
            var expectedMessageIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            await jobsStorageService.RemoveCompletedMessages(dcJobId, expectedMessageIds, CancellationToken.None);

            completedMessageRepository.Verify(x =>
                x.RemoveCompletedMessages(dcJobId, expectedMessageIds, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreCompletedMessage()
        {
            var expectedMessage = new CompletedMessage { JobId = dcJobId, MessageId = Guid.NewGuid(), Succeeded = true };

            await jobsStorageService.StoreCompletedMessage(expectedMessage, CancellationToken.None);

            completedMessageRepository.Verify(x =>
                x.StoreCompletedMessage(expectedMessage, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetJobStatus()
        {
            var expectedStatus = (true, DateTimeOffset.MinValue);
            jobStatusRepository.Setup(x => x.GetJobStatus(dcJobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStatus);

            var actualStatus = await jobsStorageService.GetJobStatus(dcJobId, CancellationToken.None);

            actualStatus.Should().Be(expectedStatus);
            jobStatusRepository.Verify(x => x.GetJobStatus(dcJobId, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreJobStatus()
        {
            var expectedStatus = (hasFailedMessages: true, endTime: DateTimeOffset.MinValue);

            await jobsStorageService.StoreJobStatus(dcJobId, expectedStatus.hasFailedMessages, expectedStatus.endTime, CancellationToken.None);

            jobStatusRepository.Verify(x =>
                x.StoreJobStatus(dcJobId, expectedStatus.hasFailedMessages, expectedStatus.endTime, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task SaveDataLocksCompletionTime()
        {
            var expectedEndTime = DateTimeOffset.MaxValue;

            await jobsStorageService.SaveDataLocksCompletionTime(dcJobId, expectedEndTime, CancellationToken.None);

            jobModelRepository.Verify(x =>
                x.SaveDataLocksCompletionTime(dcJobId, expectedEndTime, It.IsAny<CancellationToken>()));
        }
    }
}
