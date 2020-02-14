//using System;
//using System.Linq;
//using System.Runtime.CompilerServices;
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
//    public class JobModelRepository_Tests
//    {
//        private MockReliableStateManager reliableStateManager;
//        private MockTransaction transaction;

//        private Mock<IJobsDataContext> dataContext;

//        private JobModelRepository jobModelRepository;

//        private long DCJobId = 114;
//        private JobStatus ExpectedJobStatus = JobStatus.CompletedWithErrors;
//        private DateTimeOffset ExpectedEndTime = new DateTimeOffset(2017, 01, 02, 13, 14, 15, TimeSpan.FromHours(4));

//        [SetUp]
//        public void Setup()
//        {
//            using (var mocker = AutoMock.GetLoose())
//            {
//                reliableStateManager = mocker.SetupReliableStateManagerProvision();
//                transaction = mocker.SetupTransactionProvision(reliableStateManager);
//                mocker.SetupReliableStateManagerTransactionProvision(transaction);

//                dataContext = mocker.Mock<IJobsDataContext>();

//                jobModelRepository = mocker.Create<JobModelRepository>();
//            }
//        }

//        [Test]
//        public async Task ThrowawayTestGetJob()
//        {
//            var job = await jobModelRepository.GetJob(156, CancellationToken.None);
//        }
//    }

//    [TestFixture]
//    public class JobStorageService_SaveJobStatusTests
//    {
//        private MockReliableStateManager reliableStateManager;
//        private MockTransaction transaction;

//        private Mock<IJobsDataContext> dataContext;

//        private JobStorageService jobsStorageService;

//        private long DCJobId = 114;
//        private JobStatus ExpectedJobStatus = JobStatus.CompletedWithErrors;
//        private DateTimeOffset ExpectedEndTime = new DateTimeOffset(2017, 01, 02, 13, 14, 15, TimeSpan.FromHours(4));

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
//        public async Task If_A_Job_Does_Not_Exist_It_Should_Throw_Invalid_Operation_Exception()
//        {
//            var jobModelDictionary =
//                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
//                    .JobCacheKey);
//            await jobModelDictionary.AddAsync(transaction, DCJobId, null);

//            Assert.ThrowsAsync<InvalidOperationException>(async () => await jobsStorageService.SaveJobStatus(DCJobId, JobStatus.Completed, DateTimeOffset.Now, CancellationToken.None));
//        }

//        [Test]
//        public async Task Job_Should_Have_Status_Updated()
//        {
//            var jobModelDictionary =
//                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
//                    .JobCacheKey);
//            await jobModelDictionary.AddAsync(transaction, DCJobId, new JobModel());

//            await jobsStorageService.SaveJobStatus(DCJobId, ExpectedJobStatus, DateTimeOffset.Now, CancellationToken.None);

//            var actualStatus =
//                (await jobModelDictionary.TryGetValueAsync(transaction, DCJobId)).Value.Status;
//            Assert.That(actualStatus, Is.EqualTo(ExpectedJobStatus));
//        }

//        [Test]
//        public async Task Job_Should_Have_EndTime_Updated()
//        {
//            var jobModelDictionary =
//                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
//                    .JobCacheKey);
//            await jobModelDictionary.AddAsync(transaction, DCJobId, new JobModel());

//            await jobsStorageService.SaveJobStatus(DCJobId, ExpectedJobStatus, ExpectedEndTime, CancellationToken.None);

//            var actualEndTime =
//                (await jobModelDictionary.TryGetValueAsync(transaction, DCJobId)).Value.EndTime;
//            Assert.That(actualEndTime, Is.EqualTo(ExpectedEndTime));
//        }

//        [Test]
//        public async Task DataContext_Should_Be_Saved()
//        {
//            var jobModelDictionary =
//                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
//                    .JobCacheKey);
//            await jobModelDictionary.AddAsync(transaction, DCJobId, new JobModel());

//            await jobsStorageService.SaveJobStatus(DCJobId, ExpectedJobStatus, ExpectedEndTime, CancellationToken.None);

//            dataContext.Verify(x => x.SaveJobStatus(DCJobId, ExpectedJobStatus, ExpectedEndTime, CancellationToken.None));
//        }
//    }
//}