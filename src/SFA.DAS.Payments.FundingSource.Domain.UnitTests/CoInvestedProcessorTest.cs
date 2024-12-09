using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class CoInvestedProcessorTest
    {
        
        [Test]
        public void TestBothProcessorsCalled()
        {
            var requiredPayment = new RequiredPayment();
            var employerCoInvestedPaymentProcessorMock = new Mock<IEmployerCoInvestedPaymentProcessor>(MockBehavior.Strict);
            var sfaCoInvestedPaymentProcessorMock = new Mock<ISfaCoInvestedPaymentProcessor>(MockBehavior.Strict);

            var payment1 = new EmployerCoInvestedPayment {AmountDue = 100};
            var payment2 = new SfaCoInvestedPayment {AmountDue = 100};

            employerCoInvestedPaymentProcessorMock.Setup(p => p.Process(requiredPayment)).Returns(payment1).Verifiable();
            sfaCoInvestedPaymentProcessorMock.Setup(p => p.Process(requiredPayment)).Returns(payment2).Verifiable();

            ICoInvestedPaymentProcessor processor = new CoInvestedPaymentProcessor(employerCoInvestedPaymentProcessorMock.Object, sfaCoInvestedPaymentProcessorMock.Object);
            var actualPayments = processor.Process(requiredPayment);
            
            actualPayments.Should().HaveCount(2);
            actualPayments[0].Should().BeSameAs(payment1);
            actualPayments[1].Should().BeSameAs(payment2);

            employerCoInvestedPaymentProcessorMock.Verify();
            sfaCoInvestedPaymentProcessorMock.Verify();
        }

        [Test]
        public void TestEmployerOnlyProcessorsCalled()
        {
            var requiredPayment = new RequiredPayment();
            var employerCoInvestedPaymentProcessorMock = new Mock<IEmployerCoInvestedPaymentProcessor>(MockBehavior.Strict);
            var sfaCoInvestedPaymentProcessorMock = new Mock<ISfaCoInvestedPaymentProcessor>(MockBehavior.Strict);

            var payment1 = new EmployerCoInvestedPayment { AmountDue = 100 };
            var payment2 = new SfaCoInvestedPayment { AmountDue = 0 };

            employerCoInvestedPaymentProcessorMock.Setup(p => p.Process(requiredPayment)).Returns(payment1).Verifiable();
            sfaCoInvestedPaymentProcessorMock.Setup(p => p.Process(requiredPayment)).Returns(payment2).Verifiable();

            ICoInvestedPaymentProcessor processor = new CoInvestedPaymentProcessor(employerCoInvestedPaymentProcessorMock.Object, sfaCoInvestedPaymentProcessorMock.Object);
            var actualPayments = processor.Process(requiredPayment);

            actualPayments.Should().HaveCount(1);
            actualPayments[0].Should().BeSameAs(payment1);
            
            employerCoInvestedPaymentProcessorMock.Verify();
            sfaCoInvestedPaymentProcessorMock.Verify();
        }

        [Test]
        public void TestSfaOnlyProcessorsCalled()
        {
            var requiredPayment = new RequiredPayment();
            var employerCoInvestedPaymentProcessorMock = new Mock<IEmployerCoInvestedPaymentProcessor>(MockBehavior.Strict);
            var sfaCoInvestedPaymentProcessorMock = new Mock<ISfaCoInvestedPaymentProcessor>(MockBehavior.Strict);

            var payment1 = new EmployerCoInvestedPayment { AmountDue = 0 };
            var payment2 = new SfaCoInvestedPayment { AmountDue = 100 };

            employerCoInvestedPaymentProcessorMock.Setup(p => p.Process(requiredPayment)).Returns(payment1).Verifiable();
            sfaCoInvestedPaymentProcessorMock.Setup(p => p.Process(requiredPayment)).Returns(payment2).Verifiable();

            ICoInvestedPaymentProcessor processor = new CoInvestedPaymentProcessor(employerCoInvestedPaymentProcessorMock.Object, sfaCoInvestedPaymentProcessorMock.Object);
            var actualPayments = processor.Process(requiredPayment);

            actualPayments.Should().HaveCount(1);
            actualPayments[0].Should().BeSameAs(payment2);
            
            employerCoInvestedPaymentProcessorMock.Verify();
            sfaCoInvestedPaymentProcessorMock.Verify();
        }

        [Test]
        public void TestNeitherProcessorsCalled()
        {
            var requiredPayment = new RequiredPayment();
            var employerCoInvestedPaymentProcessorMock = new Mock<IEmployerCoInvestedPaymentProcessor>(MockBehavior.Strict);
            var sfaCoInvestedPaymentProcessorMock = new Mock<ISfaCoInvestedPaymentProcessor>(MockBehavior.Strict);

            var payment1 = new EmployerCoInvestedPayment { AmountDue = 0 };
            var payment2 = new SfaCoInvestedPayment { AmountDue = 0 };

            employerCoInvestedPaymentProcessorMock.Setup(p => p.Process(requiredPayment)).Returns(payment1).Verifiable();
            sfaCoInvestedPaymentProcessorMock.Setup(p => p.Process(requiredPayment)).Returns(payment2).Verifiable();

            ICoInvestedPaymentProcessor processor = new CoInvestedPaymentProcessor(employerCoInvestedPaymentProcessorMock.Object, sfaCoInvestedPaymentProcessorMock.Object);
            var actualPayments = processor.Process(requiredPayment);

            actualPayments.Should().HaveCount(0);

            employerCoInvestedPaymentProcessorMock.Verify();
            sfaCoInvestedPaymentProcessorMock.Verify();
        }

        [TestCase(FundingPlatformType.SubmitLearnerData)]
        [TestCase(FundingPlatformType.DigitalApprenticeshipService)]
        public void TestFundingPlatformTypePopulatedCorrectlyForCoInvestedFundingSourcePayments(FundingPlatformType fundingPlatformType)
        {
            var requiredPayment = new RequiredPayment
            {
                SfaContributionPercentage = 0.9m,
                FundingPlatformType = fundingPlatformType,
                AmountDue = 1000m
            };

            var validateRequiredPaymentEvent = new Mock<IValidateRequiredPaymentEvent>();

            var employerCoInvestedProcessor = new EmployerCoInvestedPaymentProcessor(validateRequiredPaymentEvent.Object);

            var sfaCoInvestedProcessor = new SfaCoInvestedPaymentProcessor(validateRequiredPaymentEvent.Object);

            var processor = new CoInvestedPaymentProcessor(employerCoInvestedProcessor, sfaCoInvestedProcessor);

            var actualPayments = processor.Process(requiredPayment);

            actualPayments.Should().HaveCount(2);

            var employerPayment = actualPayments.FirstOrDefault(x => x.Type == FundingSourceType.CoInvestedEmployer);
            employerPayment.Should().NotBeNull();
            employerPayment.FundingPlatformType.Should().Be(fundingPlatformType);
            employerPayment.AmountDue.Should().Be(100m);

            var sfaPayment = actualPayments.FirstOrDefault(x => x.Type == FundingSourceType.CoInvestedSfa);
            sfaPayment.FundingPlatformType.Should().Be(fundingPlatformType);
            sfaPayment.AmountDue.Should().Be(900m);
        }
    }
}
