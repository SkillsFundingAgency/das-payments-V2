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

using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]

    public class PeriodEndJobStatusServiceTests
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
        }

        
        
        [TestCase(JobType.PeriodEndStartJob)]
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
                    ,It.Is<bool>(b=> b == true)), Times.Once);
            mocker.Mock<IJobStatusEventPublisher>()
         
                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    ,It.Is<bool>(b=> b == false)), Times.Never);
        }


        [TestCase(JobType.PeriodEndStartJob)]
        [TestCase(JobType.PeriodEndRunJob)]
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
                    ,It.Is<bool>(b=> b == false)), Times.Once);

            mocker.Mock<IJobStatusEventPublisher>()
         
                .Verify(publisher => publisher.PeriodEndJobFinished(It.Is<JobModel>(jobModel => jobModel == job)
                    ,It.Is<bool>(b=> b == true)), Times.Never);
        }
    
        
    }
}