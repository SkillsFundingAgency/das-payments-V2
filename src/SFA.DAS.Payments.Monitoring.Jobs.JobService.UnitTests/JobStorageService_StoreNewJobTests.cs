//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Autofac.Extras.Moq;
//using Microsoft.ServiceFabric.Data.Collections;
//using Moq;
//using NUnit.Framework;
//using ServiceFabric.Mocks;
//using SFA.DAS.Payments.Monitoring.Jobs.Data;
//using SFA.DAS.Payments.Monitoring.Jobs.Model;
//using SFA.DAS.Payments.Tests.Core;

//namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.UnitTests
//{
//    [TestFixture]
//    public class JobStorageService_StoreNewJobTests
//    {
//        private MockReliableStateManager reliableStateManager;
//        private MockTransaction transaction;

//        private Mock<IJobsDataContext> dataContext;

//        private JobStorageService jobsStorageService;

//        private long DCJobId = 114;
//        private JobType ExpectedJobType = JobType.EarningsJob;

//        [SetUp]
//        public void Setup()
//        {
//            using (var mocker = AutoMock.GetLoose())
//            {
//                reliableStateManager = mocker.SetupReliableStateManagerProvision();
//                transaction = mocker.SetupTransactionProvision(reliableStateManager);
//                mocker.SetupReliableStateManagerTransactionProvision(transaction);

//                dataContext = mocker.Mock<IJobsDataContext>();

//                jobsStorageService = mocker.Create<JobStorageService>();
//            }
//        }

//        [Test]
//        public async Task Jobs_Without_DCJobId_Should_Throw_Invalid_Operation_Exception()
//        {
//            Assert.ThrowsAsync<InvalidOperationException>( async () => await jobsStorageService.StoreNewJob(new JobModel {JobType = ExpectedJobType}, CancellationToken.None));
//        }

//        [Test]
//        public async Task Jobs_Already_In_The_Cache_Should_Not_Be_Saved()
//        {
//            var jobModelDictionary =
//                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
//                    .JobCacheKey);
//            await jobModelDictionary.AddAsync(transaction, DCJobId, new JobModel {DcJobId = DCJobId, JobType = ExpectedJobType});

//            var result = await jobsStorageService.StoreNewJob(new JobModel { DcJobId = DCJobId, JobType = ExpectedJobType }, CancellationToken.None);

//            Assert.That(result, Is.False);
//        }

//        [Test]
//        public async Task Jobs_Should_Be_Saved_In_The_Cache()
//        {
//            var jobModelDictionary =
//                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
//                    .JobCacheKey);

//            await jobsStorageService.StoreNewJob(new JobModel { DcJobId = DCJobId, JobType = ExpectedJobType }, CancellationToken.None);

//            var actualDcJobId =
//                (await jobModelDictionary.TryGetValueAsync(transaction, DCJobId)).Value.DcJobId;
//            var actualJobType =
//                (await jobModelDictionary.TryGetValueAsync(transaction, DCJobId)).Value.JobType;
//            Assert.That(actualDcJobId, Is.Not.Null);
//            Assert.That(actualDcJobId, Is.EqualTo(DCJobId));
//            Assert.That(actualJobType, Is.EqualTo(ExpectedJobType));
//        }

//        [Test]
//        public async Task DataContext_Should_Not_Be_Saved_If_Job_Id_Is_Not_Zero()
//        {
//            await jobsStorageService.StoreNewJob(new JobModel { DcJobId = DCJobId, JobType = ExpectedJobType, Id = 400 }, CancellationToken.None);

//            dataContext.Verify(x => x.SaveNewJob(It.Is<JobModel>(j => j.DcJobId == DCJobId && j.JobType == ExpectedJobType), CancellationToken.None), Times.Never);
//        }

//        [Test]
//        public async Task DataContext_Should_Be_Saved()
//        {
//            await jobsStorageService.StoreNewJob(new JobModel { DcJobId = DCJobId, JobType = ExpectedJobType }, CancellationToken.None);

//            dataContext.Verify(x => x.SaveNewJob(It.Is<JobModel>(j => j.DcJobId == DCJobId && j.JobType == ExpectedJobType), CancellationToken.None));
//        }
//    }
//}