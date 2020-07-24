using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.Earnings;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class JobStatusManagerTests
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            var mockStatusService = mocker.Mock<IEarningsJobStatusService>();
            mockStatusService.Setup(x => x.ManageStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var mockScope = mocker.Mock<IUnitOfWorkScope>();
            mockScope.Setup(x => x.Resolve<IEarningsJobStatusService>())
                .Returns(mockStatusService.Object);
            mocker.Mock<IUnitOfWorkScopeFactory>()
                .Setup(x => x.Create(It.IsAny<string>()))
                .Returns(mockScope.Object);
            mocker.Mock<IJobServiceConfiguration>()
                .Setup(x => x.JobStatusInterval)
                .Returns(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public async Task Starts_Monitoring_Job()
        {
            var manager = mocker.Create<EarningsJobStatusManager>();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(10000);
            try
            {
                _ = manager.Start(cancellationTokenSource.Token).ConfigureAwait(false);
                manager.StartMonitoringJob(99, JobType.EarningsJob);
                await Task.Delay(1000, cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException) { }
            catch (OperationCanceledException) { }
            mocker.Mock<IEarningsJobStatusService>()
                .Verify(svc => svc.ManageStatus(It.Is<long>(id => id == 99), It.IsAny<CancellationToken>()));
        }
    }
}