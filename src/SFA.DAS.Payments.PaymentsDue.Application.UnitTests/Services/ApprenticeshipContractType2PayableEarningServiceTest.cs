using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Domain;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Application.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipContractType2PayableEarningServiceTest
    {
        private IApprenticeshipContractType2PayableEarningService service;
        private Mock<IApprenticeshipContractType2EarningProcessor> domainServiceMock;

        [SetUp]
        public void SetUp()
        {
            domainServiceMock = new Mock<IApprenticeshipContractType2EarningProcessor>(MockBehavior.Strict);
            service = new ApprenticeshipContractType2PayableEarningService(domainServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            domainServiceMock.Verify();
        }

        [Test]
        public void TestCreatePaymentsDue()
        {
            // arrange
            var earningEvent = new ApprenticeshipContractType2EarningEvent();
            var paymentDueEvents = new[] {new ApprenticeshipContractType2PaymentDueEvent()};
            
            domainServiceMock.Setup(d => d.HandleOnProgrammeEarning(It.IsAny<OnProgrammeEarning>(), It.IsAny<CalendarPeriod>(), It.IsAny<Learner>(), It.IsAny<LearningAim>(), It.IsAny<decimal>()))
                .Returns(paymentDueEvents)
                .Verifiable();

            // act
            var actualPaymentsDue = service.CreatePaymentsDue(earningEvent);

            // assert
            Assert.IsNotNull(actualPaymentsDue);
            Assert.AreSame(paymentDueEvents, actualPaymentsDue);
        }
    }
}
