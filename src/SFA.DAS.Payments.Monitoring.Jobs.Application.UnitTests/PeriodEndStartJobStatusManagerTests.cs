using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    public class PeriodEndStartJobStatusManagerTests
    {
        private AutoMock mocker;
        private PeriodEndStartJobStatusManager statusManager;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            var mockPaymentLogger = mocker.Mock<IPaymentLogger>();
            var mockScopeFactory =  mocker.Mock<IUnitOfWorkScopeFactory>();
            var jobServiceConfig = mocker.Mock<IJobServiceConfiguration>();
            statusManager = new PeriodEndStartJobStatusManager(mockPaymentLogger.Object, mockScopeFactory.Object, jobServiceConfig.Object );
        }

        [Test]
        public void GetJobStatusService_Returns_IPeriodEndStartJobStatusService()
        {
           var mockScope = mocker.Mock<IUnitOfWorkScope>();
          _ =   statusManager.GetJobStatusService(mockScope.Object);
          mockScope.Verify(x => x.Resolve<IPeriodEndStartJobStatusService>(), Times.Once);
        }

        [Test]
        public async Task GetCurrentJobs_Return_JobsForPeriodEndStart()
        {
            var periodEndStartJobIds = new List<long> {1};
                
            var mockStorageService = new Mock<IJobStorageService>();
            mockStorageService.Setup(x => x.GetCurrentPeriodEndStartJobs(It.IsAny<CancellationToken>()))
                .ReturnsAsync(periodEndStartJobIds);

            var jobIds = await statusManager.GetCurrentJobs(mockStorageService.Object);
            jobIds.Should().BeSameAs(periodEndStartJobIds);
            mockStorageService.Verify(x=>x.GetCurrentPeriodEndStartJobs(CancellationToken.None), Times.Once);
        }
    }
}