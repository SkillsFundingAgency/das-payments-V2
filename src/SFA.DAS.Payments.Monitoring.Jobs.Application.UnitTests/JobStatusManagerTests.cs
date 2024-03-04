using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class JobStatusManagerTests
    {
        private AutoMock mocker;
        private Mock<IJobStatusService> mockStatusService;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mockStatusService = mocker.Mock<IJobStatusService>();
            mockStatusService.Setup(x => x.ManageStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var mockScope = mocker.Mock<IUnitOfWorkScope>();
            mocker.Mock<IUnitOfWorkScopeFactory>()
                .Setup(x => x.Create(It.IsAny<string>()))
                .Returns(mockScope.Object);
            mocker.Mock<IJobServiceConfiguration>()
                .Setup(x => x.JobStatusInterval)
                .Returns(TimeSpan.FromMilliseconds(100));
            mocker.Mock<IJobStatusServiceFactory>()
                .Setup(x => x.Create(It.IsAny<IUnitOfWorkScope>(), It.IsAny<JobType>()))
                .Returns(mockStatusService.Object);
        }

        [Test]
        public async Task Checks_Status_Of_Job()
        {
            var manager = mocker.Create<JobStatusManager>();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(10000);
            try
            {
                _ = manager.Start("Test", cancellationTokenSource.Token).ConfigureAwait(false);
                await Task.Delay(200, cancellationTokenSource.Token).ConfigureAwait(false);
                manager.StartMonitoringJob(99, JobType.EarningsJob);
                await Task.Delay(1000, cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException) { }
            catch (OperationCanceledException) { }
            mockStatusService
                .Verify(svc => svc.ManageStatus(It.Is<long>(id => id == 99), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Does_Not_Check_Job_After_It_Has_Completed()
        {
            mockStatusService.Setup(x => x.ManageStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var manager = mocker.Create<JobStatusManager>();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(10000);
            try
            {
                _ = manager.Start("Test", cancellationTokenSource.Token).ConfigureAwait(false);
                await Task.Delay(200, cancellationTokenSource.Token).ConfigureAwait(false);
                manager.StartMonitoringJob(99, JobType.EarningsJob);
                await Task.Delay(1000, cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException) { }
            catch (OperationCanceledException) { }
            mockStatusService
                .Verify(svc => svc.ManageStatus(It.Is<long>(id => id == 99), It.IsAny<CancellationToken>()),Times.Once);
        }
    }
}