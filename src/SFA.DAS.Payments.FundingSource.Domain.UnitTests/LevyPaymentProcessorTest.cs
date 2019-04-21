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
    public class LevyPaymentProcessorTest
    {
        private ILevyPaymentProcessor processor;
        private Mock<ILevyBalanceService> levyBalanceServiceMock;

        [SetUp]
        public void SetUp()
        {
            levyBalanceServiceMock = new Mock<ILevyBalanceService>(MockBehavior.Strict);
            processor = new LevyPaymentProcessor(levyBalanceServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            levyBalanceServiceMock.Verify();
        }

        [Test]
        public void TestProcessPaymentWithEnoughLevyBalance()
        {
            // arrange
            var requiredPayment = new RequiredPayment
            {
                SfaContributionPercentage = .9m,
                AmountDue = 100
            };
            levyBalanceServiceMock.Setup(s => s.TryFund(100)).Returns(100).Verifiable();

            // act
            var payments = processor.Process(requiredPayment);

            // assert
            payments.Should().HaveCount(1);
            payments[0].AmountDue.Should().Be(100);
            payments[0].Type.Should().Be(FundingSourceType.Levy);
        }

        [Test]
        public void TestProcessPaymentWithZeroLevyBalance()
        {
            // arrange
            var requiredPayment = new RequiredPayment
            {
                SfaContributionPercentage = .9m,
                AmountDue = 100
            };
            levyBalanceServiceMock.Setup(s => s.TryFund(100)).Returns(0).Verifiable();

            // act
            var payments = processor.Process(requiredPayment);

            // assert
            payments.Should().BeEmpty();
        }

        [Test]
        public void TestProcessPaymentNoSfaContribution()
        {
            // arrange
            var requiredPayment = new RequiredPayment
            {
                SfaContributionPercentage = 0m,
                AmountDue = 100
            };
            levyBalanceServiceMock.Setup(s => s.TryFund(100)).Returns(100).Verifiable();

            // act
            var payments = processor.Process(requiredPayment);

            // assert
            payments.Should().HaveCount(1);
            payments[0].AmountDue.Should().Be(100);
            payments[0].Type.Should().Be(FundingSourceType.Levy);
        }

        [Test]
        public void TestProcessPaymentWithOneSfaContribution() // Small employer earnings have SFA contribution of 1
        {
            // arrange
            var requiredPayment = new RequiredPayment
            {
                SfaContributionPercentage = 1m,
                AmountDue = 100
            };
            
            // act
            var payments = processor.Process(requiredPayment);

            // assert
            payments.Should().HaveCount(0);
        }

        [Test]
        public void TestProcessPaymentWithNotEnoughLevyBalance()
        {
            // arrange
            var requiredPayment = new RequiredPayment
            {
                SfaContributionPercentage = .9m,
                AmountDue = 100
            };
            levyBalanceServiceMock.Setup(s => s.TryFund(100)).Returns(50).Verifiable();

            // act
            var payments = processor.Process(requiredPayment);

            // assert
            payments.Should().HaveCount(1);
            payments[0].AmountDue.Should().Be(50);
        }

        [Test]
        public void TestProcessRefundPayment()
        {
            // arrange
            var requiredPayment = new RequiredPayment
            {
                SfaContributionPercentage = .9m,
                AmountDue = -600
            };
            levyBalanceServiceMock.Setup(s => s.TryFund(-600)).Returns(-600).Verifiable();

            // act
            var payments = processor.Process(requiredPayment);

            // assert
            payments.Should().NotBeEmpty();
            payments[0].AmountDue.Should().Be(-600);
        }
    }
}