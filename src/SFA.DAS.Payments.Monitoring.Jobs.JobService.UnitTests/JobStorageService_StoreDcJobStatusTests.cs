using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.ServiceFabric.Data.Collections;
using Moq;
using NUnit.Framework;
using ServiceFabric.Mocks;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.UnitTests
{
    [TestFixture]
    public class JobStorageService_StoreDcJobStatusTests
    {
        private MockReliableStateManager reliableStateManager;
        private MockTransaction transaction;

        private Mock<IJobsDataContext> dataContext;

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

                dataContext = mocker.Mock<IJobsDataContext>();

                jobsStorageService = mocker.Create<JobStorageService>();
            }
        }

        [Test]
        public async Task TimedOut_Jobs_Should_Have_Status_Updated_When_Subsequent_DCJob_Confirmation_Is_Received()
        {
            var jobModelDictionary =
                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
                    .JobCacheKey);
            await jobModelDictionary.AddAsync(transaction, DCJobId,
                new JobModel {DcJobId = DCJobId, Status = JobStatus.TimedOut});

            await jobsStorageService.StoreDcJobStatus(DCJobId, true, CancellationToken.None);

            var actualStatus = (await jobModelDictionary.TryGetValueAsync(transaction, DCJobId)).Value.Status;
            Assert.That(actualStatus, Is.EqualTo(JobStatus.CompletedWithErrors));
        }

        [Test]
        public void Jobs_Not_In_The_Cache_Should_Throw_Invalid_Operation_Exception()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await jobsStorageService.StoreDcJobStatus(DCJobId, true, CancellationToken.None));
        }

        [Test]
        public async Task Jobs_Should_Have_DcJobSucceeded_Value_Updated()
        {
            var jobModelDictionary =
                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
                    .JobCacheKey);
            await jobModelDictionary.AddAsync(transaction, DCJobId, new JobModel {DcJobId = DCJobId});

            await jobsStorageService.StoreDcJobStatus(DCJobId, true, CancellationToken.None);

            var actualDcJobSucceeded =
                (await jobModelDictionary.TryGetValueAsync(transaction, DCJobId)).Value.DcJobSucceeded;
            Assert.That(actualDcJobSucceeded, Is.True);
        }

        [Test]
        public async Task Jobs_Should_Have_DcJobEndTime_Value_Updated()
        {
            var jobModelDictionary =
                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
                    .JobCacheKey);
            await jobModelDictionary.AddAsync(transaction, DCJobId, new JobModel {DcJobId = DCJobId});

            await jobsStorageService.StoreDcJobStatus(DCJobId, true, CancellationToken.None);

            var actualDcJobEndTime =
                (await jobModelDictionary.TryGetValueAsync(transaction, DCJobId)).Value.DcJobEndTime;
            Assert.That(actualDcJobEndTime, Is.Not.Null);
        }

        [Test]
        public async Task DataContext_Should_Be_Saved()
        {
            var jobModelDictionary =
                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
                    .JobCacheKey);
            await jobModelDictionary.AddAsync(transaction, DCJobId, new JobModel {DcJobId = DCJobId});

            await jobsStorageService.StoreDcJobStatus(DCJobId, true, CancellationToken.None);

            dataContext.Verify(x => x.SaveDcSubmissionStatus(DCJobId, true, CancellationToken.None));
        }
    }
}