using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData;
using ApprenticeshipModel = SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData.ApprenticeshipModel;

namespace SFA.DAS.Payments.ScheduledJobs.UnitTests
{
    [TestFixture]
    public class ApprenticeshipDataService_AlertTests
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

            var dasApprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel{ IsApproved = true },
                new ApprenticeshipModel{ PaymentStatus = PaymentStatus.Withdrawn },
                new ApprenticeshipModel{ PaymentStatus = PaymentStatus.Withdrawn },
                new ApprenticeshipModel{ PaymentStatus = PaymentStatus.Withdrawn },
                new ApprenticeshipModel{ PaymentStatus = PaymentStatus.Paused },
                new ApprenticeshipModel{ PaymentStatus = PaymentStatus.Paused },
            };

            var paymentsApprenticeships = new List<Model.Core.Entities.ApprenticeshipModel>
            {
                new Model.Core.Entities.ApprenticeshipModel{ Status = ApprenticeshipStatus.Active },
                new Model.Core.Entities.ApprenticeshipModel{ Status = ApprenticeshipStatus.Active },
                new Model.Core.Entities.ApprenticeshipModel{ Status = ApprenticeshipStatus.Active },
                new Model.Core.Entities.ApprenticeshipModel{ Status = ApprenticeshipStatus.Stopped },
                new Model.Core.Entities.ApprenticeshipModel{ Status = ApprenticeshipStatus.Paused },
                new Model.Core.Entities.ApprenticeshipModel{ Status = ApprenticeshipStatus.Paused },
                new Model.Core.Entities.ApprenticeshipModel{ Status = ApprenticeshipStatus.Paused },
                new Model.Core.Entities.ApprenticeshipModel{ Status = ApprenticeshipStatus.Paused }
            };

            commitmentsDataContext.Apprenticeship.AddRange(dasApprenticeships);
            commitmentsDataContext.SaveChanges();
            paymentsDataContext.Apprenticeship.AddRange(paymentsApprenticeships);
            paymentsDataContext.SaveChanges();

            service = new ApprenticeshipDataService(paymentsDataContext, commitmentsDataContext, telemetry.Object);
        }

        [Test]
        public void WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfDasApproved()
        {
            service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "DasApproved" && metric.Value == 1))));
        }

        [Test]
        public void WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfDasStopped()
        {
            service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "DasStopped" && metric.Value == 3))));
        }

        [Test]
        public void WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfDasPaused()
        {
            service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "DasPaused" && metric.Value == 2))));
        }

        [Test]
        public void WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfPaymentsApproved()
        {
            service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "PaymentsApproved" && metric.Value == 3))));
        }

        [Test]
        public void WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfPaymentsStopped()
        {
            service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "PaymentsStopped" && metric.Value == 1))));
        }

        [Test]
        public void WhenCallingProcessComparison_ShouldOutputTheCorrectCountOfPaymentsPaused()
        {
            service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(EventName, It.Is<Dictionary<string, double>>(metrics
                => metrics.Any(metric => metric.Key == "PaymentsPaused" && metric.Value == 4))));
        }
    }
}