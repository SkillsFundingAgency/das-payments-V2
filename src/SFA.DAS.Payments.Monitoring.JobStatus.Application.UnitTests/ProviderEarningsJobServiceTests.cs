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
            jobStarted.SubEventIds.Add((DateTimeOffset.UtcNow, Guid.NewGuid()));
            await service.JobStarted(jobStarted);
            //mocker.Mock<IJobStatusDataContext>()
            //    .Verify(dc => dc.SaveNewJob(It.Is<JobModel>(model => IsValidJobId(model,jobStarted)), 
            //        It.IsAny<CancellationToken>()),Times.Once);
        }

        private bool IsValidJobId(JobModel model, RecordStartedProcessingProviderEarningsJob jobStarted)
        {
            return model.StartTime == jobStarted.StartTime
                   && model.ProviderEarnings.All(pe => pe.DcJobId == jobStarted.JobId && pe.CollectionPeriod == jobStarted.CollectionPeriod && pe.CollectionYear == jobStarted.CollectionYear && pe.Ukprn == jobStarted.Ukprn)
                   && model.ProviderEarnings.First().DcJobId == jobStarted.JobId
                   && model.JobEvents.All(ev => ev.ParentMessageId == null && ev.MessageId == jobStarted.SubEventIds.First().EventId && ev.StartTime == jobStarted.SubEventIds.First().StartTime && ev.Status == JobStepStatus.Queued);
        }

        [Test]
        public void Uses_Correct_Job_Id_When_Saving_Provider_Earnings_Job_Steps()
        {

        }

        [Test]
        public async Task Updates_Status_Of_Completed_JobStep()
        {
            var jobStep = new JobStepModel
            {
                JobId = 4321,
                StartTime = DateTimeOffset.UtcNow.AddSeconds(-10),
                Id = 1,
                Status = JobStepStatus.Queued,
                MessageId = Guid.NewGuid()
            };

            var processedPaymentMessage = new ProcessedPaymentsMessageEvent
            {
                JobId = 1234,
                Id = jobStep.MessageId,
                EndTime = DateTimeOffset.UtcNow,
                Succeeded = true,
            };

            var service = mocker.Create<ProviderEarningsJobService>();
            await service.JobStepCompleted(processedPaymentMessage);
            
        }
    }
}