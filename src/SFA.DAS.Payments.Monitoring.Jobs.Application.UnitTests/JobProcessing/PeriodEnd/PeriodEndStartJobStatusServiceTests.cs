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
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests.JobProcessing.PeriodEnd
{
    [TestFixture]
    public class PeriodEndStartJobStatusServiceTests
    {
        private AutoMock mocker;

        private JobModel job;
        private List<CompletedMessage> completedMessages;
        private List<InProgressMessage> inProgressMessages;

        private List<OutstandingJobResult> outstandingOrTimedOutJobs;

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
                EndTime = DateTimeOffset.UtcNow,
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
                .Setup(x => x.GetAverageJobCompletionTimesForInProgressJobs(It.IsAny<List<long?>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InProgressJobAverageJobCompletionTime>
                {
                    new InProgressJobAverageJobCompletionTime
                    {
                        JobId = 100,
                        AverageJobCompletionTime = 2
                    }
                });

            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetOutstandingOrTimedOutJobs(It.IsAny<JobModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(outstandingOrTimedOutJobs);
            mocker.Mock<IJobServiceConfiguration>()
                .Setup(x => x.PeriodEndStartJobTimeout)
                .Returns(TimeSpan.FromMilliseconds(2000));

        }

        [Test]
        public async Task Uses_Correct_ApplicationName()
        {
            mocker.Mock<IServiceStatusManager>()
                .Setup(x => x.IsServiceRunning(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var service = mocker.Create<PeriodEndStartJobStatusService>();

            var result =
                await service.ManageStatus(job.DcJobId.Value, CancellationToken.None)
                    .ConfigureAwait(false);

            mocker.Mock<IServiceStatusManager>()
                .Verify(x => x.IsServiceRunning(It.Is<string>(name => name.Equals(ServiceNames.DataLocksApprovals.ApplicationName)),It.IsAny<string>()));
        }

        [Test]
        public async Task Uses_Correct_ServiceName()
        {
            mocker.Mock<IServiceStatusManager>()
                .Setup(x => x.IsServiceRunning(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var service = mocker.Create<PeriodEndStartJobStatusService>();

            var result =
                await service.ManageStatus(job.DcJobId.Value, CancellationToken.None)
                    .ConfigureAwait(false);

            mocker.Mock<IServiceStatusManager>()
                .Verify(x => x.IsServiceRunning(It.IsAny<string>(),It.Is<string>(name => name.Equals(ServiceNames.DataLocksApprovals.ServiceName))));
        }

        [Test]
        public async Task Completes_After_Reference_Data_Services_Are_Disabled()
        {
            mocker.Mock<IServiceStatusManager>()
                .Setup(x => x.IsServiceRunning(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var service = mocker.Create<PeriodEndStartJobStatusService>();

            var result =
                await service.ManageStatus(job.DcJobId.Value, CancellationToken.None)
                    .ConfigureAwait(false);

            result.Should().BeTrue();
        }

        [Test]
        public async Task Saves_Status_After_Reference_Data_Services_Are_Disabled()
        {
            mocker.Mock<IServiceStatusManager>()
                .Setup(x => x.IsServiceRunning(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var service = mocker.Create<PeriodEndStartJobStatusService>();

            await service.ManageStatus(job.DcJobId.Value, CancellationToken.None)
                    .ConfigureAwait(false);
            
            mocker.Mock<IJobStorageService>()
                .Verify(x => x.SaveJobStatus(It.Is<long>(id => id == job.DcJobId.Value),It.Is<JobStatus>(status => status == JobStatus.Completed),It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),Times.Once);
        }

        [Test]
        public async Task Publishes_Completed_Event_After_Reference_Data_Services_Are_Disabled()
        {
            mocker.Mock<IServiceStatusManager>()
                .Setup(x => x.IsServiceRunning(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var service = mocker.Create<PeriodEndStartJobStatusService>();

            await service.ManageStatus(job.DcJobId.Value, CancellationToken.None)
                .ConfigureAwait(false);

            mocker.Mock<IJobStatusEventPublisher>()
                .Verify(x => x.PeriodEndJobFinished(It.IsAny<JobModel>(), It.Is<bool>(status => status )), Times.Once);
        }

        [Test]
        public async Task Does_Not_Complete_If_Services_Are_Still_Running()
        {
            mocker.Mock<IServiceStatusManager>()
                .Setup(x => x.IsServiceRunning(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var service = mocker.Create<PeriodEndStartJobStatusService>();

            var result =
                await service.ManageStatus(job.DcJobId.Value, CancellationToken.None)
                    .ConfigureAwait(false);

            result.Should().BeFalse();
        }

        [Test]
        public async Task Times_Out_If_Services_Not_Disabled()
        {
            job.StartTime = DateTimeOffset.UtcNow;
            mocker.Mock<IJobServiceConfiguration>()
                .Setup(x => x.PeriodEndStartJobTimeout)
                .Returns(TimeSpan.FromMilliseconds(500));
            mocker.Mock<IServiceStatusManager>()
                .Setup(x => x.IsServiceRunning(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var service = mocker.Create<PeriodEndStartJobStatusService>();
            Thread.Sleep(500);
            var result =
                await service.ManageStatus(job.DcJobId.Value, CancellationToken.None)
                    .ConfigureAwait(false);

            result.Should().BeTrue();

            mocker.Mock<IJobStorageService>()
                .Verify(x => x.SaveJobStatus(It.Is<long>(id => id == job.DcJobId.Value), It.Is<JobStatus>(status => status == JobStatus.TimedOut), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        private static bool EndTimeIsNearUtcNow(DateTimeOffset endTime)
        {
            var utcNow = DateTimeOffset.UtcNow;

            return endTime >= utcNow.AddSeconds(-1) && endTime <= utcNow.AddSeconds(1);
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