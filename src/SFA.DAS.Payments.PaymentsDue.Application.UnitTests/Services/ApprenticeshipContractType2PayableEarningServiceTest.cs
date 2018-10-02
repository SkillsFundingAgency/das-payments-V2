using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Application.Services;
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
            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Learner = new Learner(),
                LearningAim = new LearningAim(),
                CollectionPeriod = new CalendarPeriod("1819R01"),
                OnProgrammeEarnings = new ReadOnlyCollection<OnProgrammeEarning>(new OnProgrammeEarning[]
                {
                    new OnProgrammeEarning()
                })
            };
            var paymentDueEvents = new[] {new ApprenticeshipContractType2PaymentDueEvent()};
            
            domainServiceMock.Setup(d => d.HandleOnProgrammeEarning(It.IsAny<OnProgrammeEarning>(), It.IsAny<CalendarPeriod>(), It.IsAny<Learner>(), It.IsAny<LearningAim>(), It.IsAny<decimal>()))
                .Returns(paymentDueEvents)
                .Verifiable();

            // act
            var actualPaymentsDue = service.CreatePaymentsDue(earningEvent);

            // assert
            Assert.IsNotNull(actualPaymentsDue);
            Assert.AreEqual(1, actualPaymentsDue.Length);
            Assert.AreSame(paymentDueEvents[0], actualPaymentsDue[0]);
        }
    }
}
