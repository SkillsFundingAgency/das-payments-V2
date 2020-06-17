using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class ApprenticeshipDataService_NoAlertTests
    {
        private IPaymentsDataContext paymentsDataContext;
        private ICommitmentsDataContext commitmentsDataContext;
        private Mock<ITelemetry> telemetry;
        private ApprenticeshipDataService service;

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
                new Model.Core.Entities.ApprenticeshipModel{ Status = ApprenticeshipStatus.Stopped },
                new Model.Core.Entities.ApprenticeshipModel{ Status = ApprenticeshipStatus.Stopped },
                new Model.Core.Entities.ApprenticeshipModel{ Status = ApprenticeshipStatus.Stopped },
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
        public async Task WhenCallingProcessComparison_ShouldNotOutputWhenThereIsNoDiscrepancy()
        {
            await service.ProcessComparison();
            telemetry.Verify(x => x.TrackEvent(It.IsAny<string>(), It.IsAny<Dictionary<string, double>>()), Times.Never);
        }
    }
}