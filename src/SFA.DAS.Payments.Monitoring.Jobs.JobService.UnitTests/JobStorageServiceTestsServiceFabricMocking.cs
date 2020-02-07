using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Moq;
using NUnit.Framework;
using ServiceFabric.Mocks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.UnitTests
{
    [TestFixture]
    public class JobStorageServiceTestsServiceFabricMocking
    {
        private MockReliableStateManager reliableStateManager;
        private MockTransaction transaction;

        private Mock<IReliableStateManagerProvider> reliableStateManagerProvider;
        private Mock<IReliableStateManagerTransactionProvider> reliableStateManagerTransactionProvider;
        private Mock<IJobsDataContext> jobsDataContext;
        private Mock<IPaymentLogger> paymentLogger;

        private JobStorageService jobsStorageService;

        private long DCJobId = 114;

        [SetUp]
        public void Setup()
        {
            reliableStateManager = new MockReliableStateManager();
            reliableStateManagerProvider = new Mock<IReliableStateManagerProvider>();
            reliableStateManagerProvider.Setup(x => x.Current).Returns(reliableStateManager);

            transaction = new MockTransaction(reliableStateManager, 100112);

            reliableStateManagerTransactionProvider = new Mock<IReliableStateManagerTransactionProvider>();
            reliableStateManagerTransactionProvider.Setup(x => x.Current).Returns(transaction);

            jobsDataContext = new Mock<IJobsDataContext>();
            paymentLogger = new Mock<IPaymentLogger>();

            jobsStorageService = new JobStorageService(
                reliableStateManagerProvider.Object,
                reliableStateManagerTransactionProvider.Object,
                jobsDataContext.Object,
                paymentLogger.Object
            );
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