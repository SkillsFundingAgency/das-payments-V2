using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.ServiceFabric.Data.Collections;
using NUnit.Framework;
using ServiceFabric.Mocks;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.UnitTests
{
    [TestFixture]
    public class JobStorageServiceTestsServiceFabricMocking
    {
        private MockReliableStateManager reliableStateManager;
        private MockTransaction transaction;

        private JobStorageService jobsStorageService;

        private long DCJobId = 114;

        [SetUp]
        public void Setup()
        {
            using (var mocker = AutoMock.GetLoose())
            {
                reliableStateManager = mocker.SetupReliableStateManagerProvision();
                transaction = mocker.SetupTransactionProvision(reliableStateManager);
                mocker.SetupReliableStateManagerTransactionProvision(transaction);

                jobsStorageService = mocker.Create<JobStorageService>();
            }
        }

        [Test]
        public async Task TimedOut_Jobs_Should_Have_Status_Updated_When_Subsequent_DCJob_Confirmation_Is_Received()
        {
            var dictionary = await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService.JobCacheKey);
            await dictionary.AddAsync(transaction, DCJobId, new JobModel{ DcJobId = DCJobId, Status = JobStatus.TimedOut });

            await jobsStorageService.StoreDcJobStatus(DCJobId, true, CancellationToken.None);

            var actualStatus = (await dictionary.TryGetValueAsync(transaction, DCJobId)).Value.Status;
            Assert.That(actualStatus, Is.EqualTo(JobStatus.CompletedWithErrors));
        }
    }
}