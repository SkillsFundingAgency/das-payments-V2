using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.JobStatus.Application.Data;
using SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Model;
using SFA.DAS.Payments.Monitoring.JobStatus.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.UnitTests
{
    [TestFixture]
    public class ProviderEarningsJobServiceTests
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Mock<IJobStatusDataContext>()
                .Setup(x => x.GetJobIdFromDcJobId(It.IsAny<long>()))
                .Returns(Task.FromResult<long>(99));
        }

        [Test]
        public async Task Stores_New_Jobs()
        {
            var service = mocker.Create<ProviderEarningsJobService>();
            var jobStarted = new RecordStartedProcessingProviderEarningsJob
            {
                CollectionPeriod = 1,
                CollectionYear = 1819,
                JobId = 1,
                Ukprn = 9999,
                IlrSubmissionTime = DateTime.UtcNow.AddMinutes(-20),
                StartTime = DateTimeOffset.UtcNow,
            };
            mocker.Mock<IJobStatusDataContext>()
                .Setup(dc => dc.SaveNewProviderEarningsJob(
                    It.IsAny<(long DcJobId, DateTimeOffset StartTime, byte CollectionPeriod, short CollectionYear, long
                        Ukprn, DateTime ilrSubmissionTime, List<(DateTimeOffset StartTime, Guid MessageId)>
                        GeneratedMessages)>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<JobModel>(new JobModel {Id = 1}));
            jobStarted.SubEventIds.Add((DateTimeOffset.UtcNow, Guid.NewGuid()));
            await service.JobStarted(jobStarted);
            mocker.Mock<IJobStatusDataContext>()
                .Verify(dc => dc.SaveNewProviderEarningsJob(
                    It.Is<(long DcJobId, DateTimeOffset StartTime, byte CollectionPeriod, short CollectionYear, long Ukprn, DateTime ilrSubmissionTime, List<(DateTimeOffset StartTime, Guid MessageId)> GeneratedMessages)>(
                        details => details.CollectionPeriod == jobStarted.CollectionPeriod && 
                                   details.CollectionYear == jobStarted.CollectionYear &&
                                   details.DcJobId == jobStarted.JobId && 
                                   details.StartTime == jobStarted.StartTime && 
                                   details.Ukprn == jobStarted.Ukprn &&
                                   details.ilrSubmissionTime == jobStarted.IlrSubmissionTime), 
                    It.IsAny<CancellationToken>()), 
                    Times.Once);
        }

        private bool IsValidJobId(JobModel model, RecordStartedProcessingProviderEarningsJob jobStarted)
        {
            return model.StartTime == jobStarted.StartTime
                   && model.ProviderEarnings.All(pe => pe.DcJobId == jobStarted.JobId && pe.CollectionPeriod == jobStarted.CollectionPeriod && pe.CollectionYear == jobStarted.CollectionYear && pe.Ukprn == jobStarted.Ukprn)
                   && model.ProviderEarnings.First().DcJobId == jobStarted.JobId
                   && model.JobEvents.All(ev => ev.ParentMessageId == null && ev.MessageId == jobStarted.SubEventIds.First().EventId && ev.StartTime == jobStarted.SubEventIds.First().StartTime && ev.Status == JobStepStatus.Queued);
        }
    }
}