//using System.Collections.Generic;
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
//    public class JobStorageService_GetCurrentJobsTests
//    {
//        private MockReliableStateManager reliableStateManager;
//        private MockTransaction transaction;

//        private JobStorageService jobsStorageService;

//        private List<JobModel> jobs;

//        [SetUp]
//        public void Setup()
//        {
//            using (var mocker = AutoMock.GetLoose())
//            {
//                reliableStateManager = mocker.SetupReliableStateManagerProvision();
//                transaction = mocker.SetupTransactionProvision(reliableStateManager);
//                mocker.SetupReliableStateManagerTransactionProvision(transaction);

//                jobsStorageService = mocker.Create<JobStorageService>();
//            }

//            jobs = new List<JobModel>
//            {
//                new JobModel{ DcJobId = 112, JobType = JobType.EarningsJob, Status = JobStatus.InProgress },
//                new JobModel{ DcJobId = 114, JobType = JobType.PeriodEndRunJob, Status = JobStatus.InProgress },
//                new JobModel{ DcJobId = 116, JobType = JobType.PeriodEndStopJob, Status = JobStatus.TimedOut }
//            };
//        }

//        [Test]
//        public async Task The_Correct_Amount_Of_Current_Jobs_Should_Be_Returned()
//        {
//            var jobModelDictionary =
//                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
//                    .JobCacheKey);
//            jobs.ForEach(async x => await jobModelDictionary.AddAsync(transaction, x.DcJobId.Value, x));

//            var actualJobs = await jobsStorageService.GetCurrentJobs(CancellationToken.None);

//            Assert.That(actualJobs.Count, Is.EqualTo(2));
//        }

//        [TestCase(112, true)]
//        [TestCase(114, true)]
//        [TestCase(116, false)]
//        public async Task The_Correct_Current_Jobs_Should_Be_Returned(long dcJobId, bool expected)
//        {
//            var jobModelDictionary =
//                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
//                    .JobCacheKey);
//            jobs.ForEach(async x => await jobModelDictionary.AddAsync(transaction, x.DcJobId.Value, x));

//            var actualJobs = await jobsStorageService.GetCurrentJobs(CancellationToken.None);

//            if(expected)
//            {
//                Assert.That(actualJobs.Contains(dcJobId));
//            }
//            else
//            {
//                Assert.That(!actualJobs.Contains(dcJobId));
//            }
//        }
//    }

//    [TestFixture]
//    public class JobStorageService_GetInProgressMessagesCollection
//    {
//        private MockReliableStateManager reliableStateManager;
//        private MockTransaction transaction;

//        private JobStorageService jobsStorageService;

//        private List<JobModel> jobs;

//        [SetUp]
//        public void Setup()
//        {
//            using (var mocker = AutoMock.GetLoose())
//            {
//                reliableStateManager = mocker.SetupReliableStateManagerProvision();
//                transaction = mocker.SetupTransactionProvision(reliableStateManager);
//                mocker.SetupReliableStateManagerTransactionProvision(transaction);

//                jobsStorageService = mocker.Create<JobStorageService>();
//            }

//            jobs = new List<JobModel>
//            {
//                new JobModel{ DcJobId = 112, JobType = JobType.EarningsJob, Status = JobStatus.InProgress },
//                new JobModel{ DcJobId = 114, JobType = JobType.PeriodEndRunJob, Status = JobStatus.InProgress },
//                new JobModel{ DcJobId = 116, JobType = JobType.PeriodEndStopJob, Status = JobStatus.TimedOut }
//            };
//        }

//        [Test]
//        public async Task The_Correct_Amount_Of_Current_Jobs_Should_Be_Returned()
//        {
//            var jobModelDictionary =
//                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
//                    .JobCacheKey);
//            jobs.ForEach(async x => await jobModelDictionary.AddAsync(transaction, x.DcJobId.Value, x));

//            var actualJobs = await jobsStorageService.GetCurrentJobs(CancellationToken.None);

//            Assert.That(actualJobs.Count, Is.EqualTo(2));
//        }

//        [TestCase(112, true)]
//        [TestCase(114, true)]
//        [TestCase(116, false)]
//        public async Task The_Correct_Current_Jobs_Should_Be_Returned(long dcJobId, bool expected)
//        {
//            var jobModelDictionary =
//                await reliableStateManager.GetOrAddAsync<IReliableDictionary2<long, JobModel>>(JobStorageService
//                    .JobCacheKey);
//            jobs.ForEach(async x => await jobModelDictionary.AddAsync(transaction, x.DcJobId.Value, x));

//            var actualJobs = await jobsStorageService.GetCurrentJobs(CancellationToken.None);

//            if (expected)
//            {
//                Assert.That(actualJobs.Contains(dcJobId));
//            }
//            else
//            {
//                Assert.That(!actualJobs.Contains(dcJobId));
//            }
//        }
//    }
//}