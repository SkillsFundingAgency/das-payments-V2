using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Domain.Services;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class PaymentProcessorTest
    {
        private Mock<ILevyPaymentProcessor> levyPaymentProcessorMock;
        private Mock<ICoInvestedPaymentProcessor> coInvestedPaymentProcessorMock;
        private IPaymentProcessor processor;

        [SetUp]
        public void SetUp()
        {
            levyPaymentProcessorMock = new Mock<ILevyPaymentProcessor>(MockBehavior.Strict);
            coInvestedPaymentProcessorMock = new Mock<ICoInvestedPaymentProcessor>(MockBehavior.Strict);
            processor = new PaymentProcessor(levyPaymentProcessorMock.Object, coInvestedPaymentProcessorMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            levyPaymentProcessorMock.Verify();
            coInvestedPaymentProcessorMock.Verify();
        }

        [Test]
        public void TestLevyOnlyCall()
        {
            // arrange
            var levyPayment = new FundingSourcePayment {AmountDue = 100};
            var requiredPayment = new RequiredPayment {AmountDue = 100};

            levyPaymentProcessorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>())).Returns(new[] {levyPayment}).Verifiable();

            // act
            var actualPayments = processor.Process(requiredPayment);

            // assert
            actualPayments.Should().NotBeNull();
            actualPayments.Should().HaveCount(1);
            actualPayments[0].Should().BeSameAs(levyPayment);
        }

        [Test]
        public void TestLevyAndCoInvestedCall()
        {
            // arrange
            var levyPayment = new FundingSourcePayment {AmountDue = 50};
            var coInvestedPayment = new FundingSourcePayment {AmountDue = 50};
            var requiredPayment = new RequiredPayment {AmountDue = 100};

            levyPaymentProcessorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>())).Returns(new[] {levyPayment}).Verifiable();
            coInvestedPaymentProcessorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>())).Returns(new[] {coInvestedPayment}).Verifiable();

            // act
            var actualPayments = processor.Process(requiredPayment);

            // assert
            actualPayments.Should().NotBeNull();
            actualPayments.Should().HaveCount(2);
            actualPayments[0].Should().BeSameAs(levyPayment);
            actualPayments[1].Should().BeSameAs(coInvestedPayment);
        }
    }
}