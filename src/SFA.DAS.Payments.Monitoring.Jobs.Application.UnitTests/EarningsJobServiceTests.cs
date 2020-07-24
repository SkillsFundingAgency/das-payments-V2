using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.Earnings;
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
            jobSteps = new List<JobStepModel>();
            mocker = AutoMock.GetLoose();
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.StoreNewJob(It.IsAny<JobModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            object job = new JobModel { Id = jobStep.JobId, DcJobId = jobMessageStatus.JobId };
            mocker.Mock<IMemoryCache>()
                .Setup(cache => cache.TryGetValue(It.IsAny<string>(), out job))
                .Returns(true);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetInProgressMessages(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InProgressMessage>());

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

            var service = mocker.Create<EarningsJobService>();
            await service.RecordNewJob(jobStarted,default(CancellationToken));

            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreNewJob(
                    It.Is<JobModel>(job =>
                        job.StartTime == jobStarted.StartTime
                        && job.JobType == JobType.EarningsJob
                        && job.Status == JobStatus.InProgress && job.DcJobId == jobStarted.JobId
                        && job.CollectionPeriod == jobStarted.CollectionPeriod
                        && job.AcademicYear == jobStarted.CollectionYear
                        && job.IlrSubmissionTime == jobStarted.IlrSubmissionTime
                        && job.Ukprn == jobStarted.Ukprn), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Stores_Earning_Job_Inprogress_Messages()
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
            var jobStarted = new RecordJobAdditionalMessages
            {
                GeneratedMessages = new List<GeneratedMessage> { generatedMessageA, generatedMessageB },
                JobId = 1,
            };

            var service = mocker.Create<EarningsJobService>();
            await service.RecordNewJobAdditionalMessages(jobStarted, CancellationToken.None);

            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreInProgressMessages(It.Is<long>(jobId => jobId == jobStarted.JobId), It.Is<List<InProgressMessage>>(identifiers =>
                    identifiers.Count == 2 &&
                    identifiers.Exists(inProgress => inProgress.MessageId == generatedMessageA.MessageId) &&
                    identifiers.Exists(inProgress => inProgress.MessageId == generatedMessageB.MessageId)), It.IsAny<CancellationToken>()), Times.Once);
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
            var jobStarted = new RecordEarningsJob
            {
                CollectionPeriod = 1,
                CollectionYear = 1819,
                JobId = 1,
                Ukprn = 9999,
                IlrSubmissionTime = DateTime.UtcNow.AddMinutes(-20),
                StartTime = DateTimeOffset.UtcNow,
                GeneratedMessages = new List<GeneratedMessage> { generatedMessageA, generatedMessageB }
            };

            var service = mocker.Create<EarningsJobService>();
            await service.RecordNewJob(jobStarted, default(CancellationToken));

            mocker.Mock<IJobStorageService>()
                .Verify(x => x.StoreInProgressMessages(It.Is<long>(jobId => jobId == jobStarted.JobId), It.Is<List<InProgressMessage>>(identifiers =>
                    identifiers.Count == 2 &&
                    identifiers.Exists(inProgress => inProgress.MessageId == generatedMessageA.MessageId) &&
                    identifiers.Exists(inProgress => inProgress.MessageId == generatedMessageB.MessageId)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Stores_Submission_Succeeded()
        {
            var submissionJobId = 1234;
            var service = mocker.Create<EarningsJobService>();
            await service.RecordDcJobCompleted(submissionJobId, true, CancellationToken.None);

            mocker.Mock<IJobStorageService>()
                .Verify(svc => svc.StoreDcJobStatus(It.Is<long>(jobId => jobId == submissionJobId), It.Is<bool>(succeeded => succeeded), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Stores_Submission_Failed()
        {
            var submissionJobId = 1234;
            var service = mocker.Create<EarningsJobService>();
            await service.RecordDcJobCompleted(submissionJobId, false, CancellationToken.None);

            mocker.Mock<IJobStorageService>()
                .Verify(svc => svc.StoreDcJobStatus(It.Is<long>(jobId => jobId == submissionJobId), It.Is<bool>(succeeded => !succeeded), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}