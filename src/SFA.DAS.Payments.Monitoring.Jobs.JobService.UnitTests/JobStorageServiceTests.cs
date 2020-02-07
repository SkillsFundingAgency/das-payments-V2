using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.UnitTests
{
    [TestFixture]
    public class JobStorageServiceTests
    {
        private Autofac.Extras.Moq.AutoMock mocker;
        private JobModel job;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            job = new JobModel
            {
                Id = 1,
                DcJobId = 99,
                StartTime = DateTimeOffset.UtcNow.AddSeconds(-10),
                Status = JobStatus.TimedOut,
                LearnerCount = 100,
                JobType = JobType.EarningsJob,
                DcJobEndTime = DateTimeOffset.Now,
                DcJobSucceeded = true,
                IlrSubmissionTime = DateTime.Now,
                Ukprn = 1234,
                AcademicYear = 1920,
                CollectionPeriod = 01
            };
        }


        [Test]
        public async Task TimedOut_Jobs_Should_Have_Status_Updated_When_Subsequent_DCJob_Confirmation_Is_Received()
        {
            var JobValue = new Microsoft.ServiceFabric.Data.ConditionalValue<JobModel>(true, job);

            var mockDictionary = mocker.Mock<IReliableDictionary2<long, JobModel>>();
            mockDictionary.Setup(md => md.TryGetValueAsync(It.IsAny<ITransaction>(), It.IsAny<long>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Microsoft.ServiceFabric.Data.ConditionalValue<JobModel>(true, job));

            var stateManager = mocker.Mock<IReliableStateManager>();
            stateManager.Setup(sm => sm.GetOrAddAsync<IReliableDictionary2<long, JobModel>>("jobs"))
                .Returns(Task.FromResult(mockDictionary.Object));

            var mockStateManagerProvider = new Mock<IReliableStateManagerProvider>();
            mockStateManagerProvider.Setup(smp => smp.Current).Returns(stateManager.Object);

            var mockStateTransactionProvider = new Mock<IReliableStateManagerTransactionProvider>();
            var mockJobDataContext = new Mock<IJobsDataContext>();
            var mockLogger = new Mock<IPaymentLogger>();

            var jobStorageService = new JobStorageService(mockStateManagerProvider.Object,
                mockStateTransactionProvider.Object, mockJobDataContext.Object, mockLogger.Object);
            
           // var jobStorageService = mocker.Create<JobStorageService>();
            await jobStorageService.StoreDcJobStatus(job.Id, true, new CancellationToken());


        }
    }
}