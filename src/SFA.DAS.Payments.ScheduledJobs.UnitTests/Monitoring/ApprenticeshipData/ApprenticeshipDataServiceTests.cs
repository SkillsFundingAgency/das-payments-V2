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
using ApprenticeshipModel = SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData.ApprenticeshipModel;

namespace SFA.DAS.Payments.ScheduledJobs.UnitTests.Monitoring.ApprenticeshipData
{
    [TestFixture]
    public class ApprenticeshipDataServiceTests
    {
        private IPaymentsDataContext paymentsDataContext;
        private ICommitmentsDataContext commitmentsDataContext;
        private Mock<ITelemetry> telemetry;
        private ApprenticeshipDataService service;

        private const string EventName = "ApprovalsReferenceDataComparisonEvent";

        [SetUp]
        public void SetUp()
        {
            telemetry = new Mock<ITelemetry>();

            commitmentsDataContext = new CommitmentsDataContext(new DbContextOptionsBuilder<CommitmentsDataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            paymentsDataContext = new PaymentsDataContext(new DbContextOptionsBuilder<PaymentsDataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var commitmentsDataContextFactory = new Mock<ICommitmentsDataContextFactory>();
            commitmentsDataContextFactory.Setup(c => c.Create())
                .Returns(commitmentsDataContext);

            var paymentsDataContextFactory = new Mock<IPaymentsDataContextFactory>();
            paymentsDataContextFactory.Setup(c => c.Create())
                .Returns(paymentsDataContext);


            var dasApprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel { IsApproved = true, Commitment = new Commitment { EmployerAndProviderApprovedOn = DateTime.Now, Approvals = 3} },
                new ApprenticeshipModel { IsApproved = false, Commitment = new Commitment { EmployerAndProviderApprovedOn = DateTime.Now, Approvals = 3} }, //assert we are using the correct new logic based on query in PV2-2215
                new ApprenticeshipModel { IsApproved = true, Commitment = new Commitment { EmployerAndProviderApprovedOn = DateTime.Now, Approvals = 7} }, //assert we include transfers

                new ApprenticeshipModel { PaymentStatus = PaymentStatus.Withdrawn, StopDate = DateTime.Now, Commitment = new Commitment { EmployerAndProviderApprovedOn = DateTime.Now } }, //expected in stopped count
                new ApprenticeshipModel { PaymentStatus = PaymentStatus.Withdrawn, StopDate = DateTime.Now, Commitment = new Commitment { EmployerAndProviderApprovedOn = DateTime.Now } }, //expected in stopped count
                new ApprenticeshipModel { PaymentStatus = PaymentStatus.Withdrawn, StopDate = DateTime.Now.AddDays(10), Commitment = new Commitment { EmployerAndProviderApprovedOn = DateTime.Now } }, //expected in stopped count
                new ApprenticeshipModel { PaymentStatus = PaymentStatus.Withdrawn, StopDate = DateTime.Now.AddDays(-31), Commitment = new Commitment { EmployerAndProviderApprovedOn = DateTime.Now } }, //not expected in stopped count (stop date too old)
                new ApprenticeshipModel { PaymentStatus = PaymentStatus.Withdrawn, Commitment = new Commitment { EmployerAndProviderApprovedOn = DateTime.Now } }, //not expected in stopped count (null stop date)
                new ApprenticeshipModel { PaymentStatus = PaymentStatus.Completed, StopDate = DateTime.Now, Commitment = new Commitment { EmployerAndProviderApprovedOn = DateTime.Now } }, //not expected in stopped count (wrong status even though stop date is now)

                new ApprenticeshipModel { PaymentStatus = PaymentStatus.Paused, IsApproved = true, PauseDate = DateTime.Now },
                new ApprenticeshipModel { PaymentStatus = PaymentStatus.Withdrawn, IsApproved = true, PauseDate = DateTime.Now },
                new ApprenticeshipModel { PaymentStatus = PaymentStatus.Paused, IsApproved = false, PauseDate = DateTime.Now },
                new ApprenticeshipModel { PaymentStatus = PaymentStatus.Paused, IsApproved = true, PauseDate = DateTime.Now.AddDays(-31) },
            };

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

            commitmentsDataContext.Apprenticeship.AddRange(dasApprenticeships);
            commitmentsDataContext.SaveChanges();
            paymentsDataContext.Apprenticeship.AddRange(paymentsApprenticeships);
            paymentsDataContext.SaveChanges();

            service = new ApprenticeshipDataService(paymentsDataContextFactory.Object, commitmentsDataContextFactory.Object, telemetry.Object);
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