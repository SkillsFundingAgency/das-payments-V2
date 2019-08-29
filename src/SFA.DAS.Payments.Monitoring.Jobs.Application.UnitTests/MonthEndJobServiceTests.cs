using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class MonthEndJobServiceTests
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
            mocker = AutoMock.GetLoose();
            var mockDataContext = mocker.Mock<IJobsDataContext>();
            mockDataContext.Setup(x => x.GetJobIdFromDcJobId(It.IsAny<long>()))
                .Returns(Task.FromResult<long>(99));
            mockDataContext.Setup(x => x.GetJobByDcJobId(It.IsAny<long>()))
                .Returns(Task.FromResult<JobModel>(new JobModel { Id = jobStep.JobId, DcJobId = jobMessageStatus.JobId }));
            jobSteps = new List<JobStepModel> { jobStep };
            mockDataContext
                .Setup(dc => dc.GetJobSteps(It.IsAny<List<Guid>>()))
                .Returns(Task.FromResult<List<JobStepModel>>(jobSteps));
            mockDataContext.Setup(dc => dc.GetJobIdFromDcJobId(It.IsAny<long>()))
                .Returns(Task.FromResult<long>(1));
            object job = new JobModel { Id = jobStep.JobId, DcJobId = jobMessageStatus.JobId };
            mocker.Mock<IMemoryCache>()
                .Setup(cache => cache.TryGetValue(It.IsAny<string>(), out job))
                .Returns(true);
        }

        //[Test]
        //public async Task Stores_New_Jobs()
        //{
        //    var jobStarted = new RecordPeriodEndStartJob()
        //    {
        //        CollectionPeriod = 1,
        //        CollectionYear = 1819,
        //        JobId = 1,
        //        StartTime = DateTimeOffset.UtcNow,
        //    };
        //    var service = mocker.Create<MonthEndJobService>();
        //    await service.RecordPeriodEndJob(jobStarted);
        //    mocker.Mock<IJobsDataContext>()
        //        .Verify(dc => dc.SaveNewJob(
        //            It.Is<JobModel>(job =>
        //                job.StartTime == jobStarted.StartTime
        //                && job.JobType == JobType.PeriodEndStartJob
        //                && job.Status == JobStatus.InProgress 
        //                && job.DcJobId == jobStarted.JobId
        //                && job.CollectionPeriod == jobStarted.CollectionPeriod
        //                && job.AcademicYear == jobStarted.CollectionYear
        //                && job.IlrSubmissionTime == null
        //                && job.Ukprn == null),
        //            It.IsAny<List<JobStepModel>>(), 
        //            It.IsAny<CancellationToken>()), 
        //            Times.Once);
        //}

        private async Task JobStepCompleted()
        {
            var service = mocker.Create<JobMessageService>();
            await service.JobMessageCompleted(jobMessageStatus);
        }
    }
}