using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
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
        private Mock<IIncentiveProcessor> incentiveEarningProcessor;

        [SetUp]
        public void SetUp()
        {
            domainServiceMock = new Mock<IApprenticeshipContractType2EarningProcessor>(MockBehavior.Strict);
            incentiveEarningProcessor = new Mock<IIncentiveProcessor>(MockBehavior.Strict);
            service = new ApprenticeshipContractType2PayableEarningService(domainServiceMock.Object, incentiveEarningProcessor.Object);
        }

        [TearDown]
        public void TearDown()
        {
            domainServiceMock.Verify();
        }

        [Test]
        public void TestCreatePaymentsDueForOnProgrammeEarning()
        {
            // arrange
            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Learner = new Learner(),
                LearningAim = new LearningAim(),
                CollectionPeriod = CollectionPeriod.CreateFromAcademicYearAndPeriod("1819", 1),
                OnProgrammeEarnings = new ReadOnlyCollection<OnProgrammeEarning>(new[]
                {
                    new OnProgrammeEarning()
                })
            };
            var paymentDueEvents = new[] {new ApprenticeshipContractType2PaymentDueEvent()};
            
            domainServiceMock.Setup(d => d.HandleOnProgrammeEarning(It.IsAny<Submission>(), It.IsAny<OnProgrammeEarning>(), It.IsAny<Learner>(), It.IsAny<LearningAim>(), It.IsAny<decimal>()))
                .Returns(paymentDueEvents)
                .Verifiable();

            // act
            var actualPaymentsDue = service.CreatePaymentsDue(earningEvent);

            // assert
            Assert.IsNotNull(actualPaymentsDue);
            Assert.AreEqual(1, actualPaymentsDue.Length);
            Assert.AreSame(paymentDueEvents[0], actualPaymentsDue[0]);
        }

        [Test]
        public void TestCreatePaymentsDueForOnIncentiveEarning()
        {
            // arrange
            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Learner = new Learner(),
                LearningAim = new LearningAim(),
                CollectionPeriod = CollectionPeriod.CreateFromAcademicYearAndPeriod("1819", 1),
                IncentiveEarnings = new ReadOnlyCollection<IncentiveEarning>(new[]
                {
                    new IncentiveEarning()
                })
            };
            var paymentDueEvents = new[] { new IncentivePaymentDueEvent() };

            incentiveEarningProcessor.Setup(d => d.HandleIncentiveEarnings(It.IsAny<Submission>(), It.IsAny<IncentiveEarning>(), It.IsAny<Learner>(), It.IsAny<LearningAim>(), It.IsAny<decimal>(), ContractType.Act2))
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
