using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData;

namespace SFA.DAS.Payments.ScheduledJobs.UnitTests.Monitoring.ApprenticeshipData
{
    [TestFixture]
    public class ApprenticeshipDataServiceTests
    {
        private IPaymentsDataContext paymentsDataContext;
        private Mock<ITelemetry> telemetry;
        private ApprenticeshipDataService service;

        private const string EventName = "ApprovalsReferenceDataComparisonEvent";

        [SetUp]
        public void SetUp()
        {
            telemetry = new Mock<ITelemetry>();

            var commitmentStats = new CommitmentStatsDto
            {
                Approved = 3,
                Paused = 1,
                Stopped = 3
            };

            var commitmentsApiClient = new Mock<ICommitmentsApiClient>();

            commitmentsApiClient.Setup(x => x.GetStats(It.IsAny<int>())).ReturnsAsync(commitmentStats);

            paymentsDataContext = new PaymentsDataContext(new DbContextOptionsBuilder<PaymentsDataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var paymentsDataContextFactory = new Mock<IPaymentsDataContextFactory>();
            paymentsDataContextFactory.Setup(c => c.Create())
                .Returns(paymentsDataContext);


            var paymentsApprenticeships = new List<Model.Core.Entities.ApprenticeshipModel>
            {
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Active, CreationDate = DateTime.Now },
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Active, CreationDate = DateTime.Now },
                new Model.Core.Entities.ApprenticeshipModel { CreationDate = DateTime.Now }, //status doesn't matter anymore, assert query mirrors this
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Active, CreationDate = DateTime.Now.AddDays(-31) },
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Stopped, StopDate = DateTime.Now },
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Stopped, StopDate = DateTime.Now.AddDays(-31) },
                new Model.Core.Entities.ApprenticeshipModel { StopDate = DateTime.Now.AddDays(-31) },
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Paused, ApprenticeshipPauses = new List<ApprenticeshipPauseModel>{ new ApprenticeshipPauseModel{ PauseDate = DateTime.Now, ResumeDate = null } }},
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Paused, ApprenticeshipPauses = new List<ApprenticeshipPauseModel>{ new ApprenticeshipPauseModel{ PauseDate = DateTime.Now, ResumeDate = null } }},
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Paused, ApprenticeshipPauses = new List<ApprenticeshipPauseModel>{ new ApprenticeshipPauseModel{ PauseDate = DateTime.Now, ResumeDate = null } }},
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Paused, CreationDate = DateTime.Now },
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Paused, ApprenticeshipPauses = new List<ApprenticeshipPauseModel>{ new ApprenticeshipPauseModel{ PauseDate = DateTime.Now, ResumeDate = DateTime.Now } }},
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Paused, ApprenticeshipPauses = new List<ApprenticeshipPauseModel>{ new ApprenticeshipPauseModel{ PauseDate = DateTime.Now.AddDays(-31), ResumeDate = null } }},
                new Model.Core.Entities.ApprenticeshipModel { Status = ApprenticeshipStatus.Inactive, ApprenticeshipPauses = new List<ApprenticeshipPauseModel>{ new ApprenticeshipPauseModel{ PauseDate = DateTime.Now, ResumeDate = null } }}
            };

            paymentsDataContext.Apprenticeship.AddRange(paymentsApprenticeships);
            paymentsDataContext.SaveChanges();

            service = new ApprenticeshipDataService(paymentsDataContextFactory.Object, commitmentsApiClient.Object, telemetry.Object);
        }

        [Test]
        public async Task WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfDasApproved()
        {
            await service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "DasApproved" && metric.Value == 3))));
        }

        [Test]
        public async Task WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfDasStopped()
        {
            await service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "DasStopped" && metric.Value == 3))));
        }

        [Test]
        public async Task WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfDasPaused()
        {
            await service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "DasPaused" && metric.Value == 1))));
        }

        [Test]
        public async Task WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfPaymentsApproved()
        {
            await service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "PaymentsApproved" && metric.Value == 4))));
        }

        [Test]
        public async Task WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfPaymentsStopped()
        {
            await service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "PaymentsStopped" && metric.Value == 1))));
        }

        [Test]
        public async Task WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfPaymentsPaused()
        {
            await service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "PaymentsPaused" && metric.Value == 3))));
        }
    }
}