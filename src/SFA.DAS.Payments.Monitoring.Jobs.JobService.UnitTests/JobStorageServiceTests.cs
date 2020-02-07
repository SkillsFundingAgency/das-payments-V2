using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.UnitTests
{
    [TestFixture]
    public class JobStorageServiceTests
    {
        private JobModel jobModel;
        private JobStorageService jobStorageService;

        [SetUp]
        public void SetUp()
        {
            using (var mocker = AutoMock.GetLoose())
            {
                jobModel = new JobModel
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

                var mockDictionary = mocker.Mock<IReliableDictionary2<long, JobModel>>();
                var stateManager = mocker.Mock<IReliableStateManager>();
                var mockStateManagerProvider = mocker.Mock<IReliableStateManagerProvider>();


                mockDictionary.Setup(md => md.TryGetValueAsync(It.IsAny<ITransaction>(), It.IsAny<long>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new ConditionalValue<JobModel>(true, jobModel));

                stateManager.Setup(sm => sm.GetOrAddAsync<IReliableDictionary2<long, JobModel>>("jobs"))
                    .Returns(Task.FromResult(mockDictionary.Object));

                mockStateManagerProvider.SetupGet(smp => smp.Current).Returns(stateManager.Object);

                jobStorageService = mocker.Create<JobStorageService>();
            }
        }

        [TestCase(JobStatus.TimedOut)]
        public async Task TimedOut_Jobs_Should_Have_Status_Updated_When_Subsequent_DCJob_Confirmation_Is_Received(JobStatus status)
        {
            await jobStorageService.StoreDcJobStatus(jobModel.Id, true, new CancellationToken());
        }
    }
}